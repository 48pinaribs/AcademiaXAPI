using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Concrete;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Models;
using AcademiaX_Data_Access.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Render (ve benzeri PaaS'lar) konteynere dışarıdan hangi portu dinleyeceğini
// PORT ortam değişkeniyle söyler; Kestrel'i buna göre bağlıyoruz. Yerelde (PORT
// tanımlı değilken) launchSettings.json'daki profiller değişmeden çalışmaya devam eder.
var renderPort = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(renderPort))
{
	builder.WebHost.UseUrls($"http://0.0.0.0:{renderPort}");
}

// Add services to the container.

builder.Services.AddControllers(options =>
{
	// DTO'larda sadece açıkça [Required] işaretlenen alanlar zorunlu olsun.
	// Varsayılan davranışta (false) non-nullable reference type (string vb.) her alan,
	// [Required] eklenmemiş olsa bile zımnen zorunlu sayılıyor — bu da Image/Address/Email/
	// PhoneNumber gibi bilerek opsiyonel bırakılmış alanları da zorunlu kılıp gerçek istekleri
	// (ör. resimsiz kayıt, kısmi profil güncelleme) 400 ile reddediyordu.
	options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});
builder.Services.AddMemoryCache(); // GtfsService'in durak/sefer verisini cache'lemesi için
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")); });
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
	// Identity'nin varsayılan (zaten güçlü) şifre politikasını koru.
	options.Password.RequiredLength = 8;
})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

// JWT Bearer authentication: token'lar Login sırasında üretiliyor ama şu ana kadar
// hiçbir middleware bu token'ları doğrulamıyordu, yani [Authorize] fiilen işe yaramıyordu.
var jwtKey = builder.Configuration.GetValue<string>("SecretKey:jwtKey");
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
{
	throw new InvalidOperationException(
		"SecretKey:jwtKey en az 32 karakter uzunluğunda, güçlü/rastgele bir değer olmalı. " +
		"Değeri appsettings.json yerine 'dotnet user-secrets' veya ortam değişkeni ile sağlayın.");
}

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	// Development'ta HTTPS metadata zorunluluğunu launchSettings http profiliyle uyumlu tutuyoruz.
	options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
	options.SaveToken = true;
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ClockSkew = TimeSpan.FromMinutes(1)
	};
});
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserService>();//DI
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IGtfsService, GtfsService>();
builder.Services.AddScoped<IAnnouncementService, AnnouncementService>();
builder.Services.AddSwaggerGen(options =>
{
	// Swagger UI'dan "Authorize" ile Bearer token girip korumalı endpoint'leri test edebilmek için.
	options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "JWT token'ı 'Bearer {token}' formatında girin."
	});
	options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
			},
			Array.Empty<string>()
		}
	});
});
builder.Services.AddScoped(typeof(ApiResponse));

// CORS'u tanımla. İzinli origin listesi appsettings/ortam değişkeninden okunur
// (Cors:AllowedOrigins), böylece prod'da Vercel domain'ini kod değiştirmeden
// eklemek mümkün olur. Hiç ayarlanmamışsa yerel geliştirme origin'ine düşer.
var configuredOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
var allowedOrigins = (configuredOrigins is { Length: > 0 })
	? configuredOrigins
	: new[] { "http://localhost:3000" };

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		builder =>
		{
			builder.WithOrigins(allowedOrigins)
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});


var app = builder.Build();

// Roller artık her Register isteğinde değil, uygulama başlangıcında bir kere seed edilir
// (önceki kod bunu her kayıt isteğinde kontrol ediyordu — gereksiz ve dağınıktı).
using (var scope = app.Services.CreateScope())
{
	// Bekleyen migration'ları uygulamaya başlarken otomatik uygula — Render gibi
	// tek kullanımlık konteynerlerde "önce elle dotnet ef database update çalıştır"
	// adımı yok, deploy'un kendisi migration'ı da taşımalı.
	var dbContext = scope.ServiceProvider.GetRequiredService<AcademiaX_Data_Access.Context.ApplicationDbContext>();
	await dbContext.Database.MigrateAsync();

	var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
	foreach (var roleName in Enum.GetNames(typeof(UserType)))
	{
		if (!await roleManager.RoleExistsAsync(roleName))
		{
			await roleManager.CreateAsync(new IdentityRole(roleName));
		}
	}

	// Kampüs ulaşım (dolmuş) verisi — tablo boşsa bir kere örnek veriyle doldurulur.
	await GtfsSeeder.SeedAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
	// Prod'da yakalanmamış exception'ların stack trace/iç detay sızdırmasını engelle.
	app.UseExceptionHandler("/error");

	// Render (ve benzeri PaaS'lar) TLS'i kendi edge/proxy katmanında sonlandırıp
	// konteynere düz HTTP ile iletir. UseHttpsRedirection bunu her istekte tekrar
	// HTTPS'e yönlendirmeye çalışıp sonsuz redirect/CORS preflight hatasına yol
	// açar; X-Forwarded-Proto'ya güvenerek şemayı doğru okumasını sağlıyoruz.
	var forwardedHeadersOptions = new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	};
	// Render'ın proxy zinciri bilinen sabit bir IP/ağ olmadığı için varsayılan
	// KnownProxies/KnownNetworks kısıtlamasını (yalnızca loopback'e güvenme) kaldırıyoruz
	// — aksi halde ForwardedHeaders middleware'i Render'ın proxy'sinden gelen
	// X-Forwarded-Proto header'ını güvenilmez sayıp yok sayar.
	forwardedHeadersOptions.KnownNetworks.Clear();
	forwardedHeadersOptions.KnownProxies.Clear();
	app.UseForwardedHeaders(forwardedHeadersOptions);
}

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

// Sıra önemli: CORS, routing/endpoint eşleştirmesinden ve
// authentication/authorization'dan ÖNCE gelmeli, aksi halde
// bazı yanıtlarda (özellikle 401/403) CORS header'ları eklenmez.
app.UseCors("AllowReact");

app.UseAuthentication(); // Token'ı doğrulayıp HttpContext.User'ı doldurur (daha önce hiç çağrılmıyordu).
app.UseAuthorization();

app.MapControllers();
app.Map("/error", (HttpContext _) => Results.Problem(statusCode: 500, title: "Beklenmeyen bir hata oluştu."));

app.Run();
