using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Concrete;
using AcademiaX_Core.Configuration;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationDbContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); });
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddScoped<IUserService, UserService>();//DI
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IGtfsService, GtfsService>();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddScoped(typeof(ApiResponse));

builder.Services.Configure<GtfsSettings>(
	builder.Configuration.GetSection("GtfsSettings"));


// CORS'u tan�mla
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowReact",
		builder =>
		{
			builder.WithOrigins("http://localhost:3000")
				   .AllowAnyHeader()
				   .AllowAnyMethod();
		});
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors("AllowReact");

app.Run();
