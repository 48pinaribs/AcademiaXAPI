using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Enums;
using AcademiaX_Data_Access.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Concrete
{
	public class UserService : IUserService
	{
		private readonly ApplicationDbContext _context;
		private readonly IMapper _mapper;
		private readonly ApiResponse _response;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private string secretKey;

		public UserService(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager, ApiResponse response, IMapper mapper, IConfiguration configuration, ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_response = response;
			_mapper = mapper;
			_context = context;
			secretKey = configuration.GetValue<string>("SecretKey:jwtKey");
		}

		/* Genel Yapı:
              Kullanıcı adı veritabanında var mı diye bakılıyor
              Şifre doğru mu kontrol ediliyor
              Rol bilgisi alınıyor
              JWT token oluşturuluyor(Claimlerle)
              Sonuç (LoginResponseModel) geri döndürülüyor */

		public async Task<ApiResponse> Login(LoginRequestDTO model)
		{
			ApplicationUser userFromDb = _context.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
			if (userFromDb != null)
			{
				bool isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
				if (!isValid)
				{
					_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
					_response.ErrorMessages.Add("Your entry information is not correct");
					_response.IsSuccess = false;
					return _response;
				}
				var role = await _userManager.GetRolesAsync(userFromDb);
				JwtSecurityTokenHandler tokenHandler = new();
				byte[] key = Encoding.ASCII.GetBytes(secretKey);

				SecurityTokenDescriptor tokenDescriptor = new()
				{
					Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
					{
						new Claim(ClaimTypes.NameIdentifier, userFromDb.Id),
						new Claim(ClaimTypes.Email, userFromDb.Email),
						new Claim(ClaimTypes.Role, role.FirstOrDefault() == null ? "Student" : role.FirstOrDefault())
					}),
					Expires = DateTime.UtcNow.AddDays(1),
					SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)

				};

				SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

				LoginResponseModel _model = new()
				{
					Email = userFromDb.Email,
					Token = tokenHandler.WriteToken(token),
				};
				_response.Result = _model;
				_response.IsSuccess = true;
				_response.StatusCode = System.Net.HttpStatusCode.OK;
				return _response;

			}
			_response.IsSuccess = false;
			_response.ErrorMessages.Add("Ooops! something went wrong");
			return _response;


		}



		
		public  async Task<ApiResponse> Register(RegisterRequestDTO model)
		{
			var userFromDb = _context.ApplicationUsers.FirstOrDefault(x=>x.UserName.ToLower() == model.UserName.ToLower());
			if (userFromDb != null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("UserName already exist");
				return _response;
			}

			//var newUser = _mapper.Map<ApplicationUser>(model);

			ApplicationUser newUser = new()
			{
				UserName = model.UserName,
				Email = model.Email,
				UserType = Enum.TryParse<UserType>(model.UserType, true, out var userType) ? userType : null,
				PasswordHash = model.Password,
				PhoneNumber = model.PhoneNumber,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Image = model.Image,

			};

			var result = await _userManager.CreateAsync(newUser, model.Password); //ASP.NET Core Identity sisteminde yeni bir kullanıcıyı veritabanına eklemek için kullanılır.



			//ASP.NET Core Identity sisteminde kayıt olan kullanıcıya uygun rol atamak için kullanılıyor.
			if (result.Succeeded)
			{
				var isTrue = _roleManager.RoleExistsAsync(UserType.Administrator.ToString()).GetAwaiter().GetResult();
				if (!_roleManager.RoleExistsAsync(UserType.Administrator.ToString()).GetAwaiter().GetResult())
				{
					await _roleManager.CreateAsync(new IdentityRole(UserType.Administrator.ToString()));
					await _roleManager.CreateAsync(new IdentityRole(UserType.Student.ToString()));
					await _roleManager.CreateAsync(new IdentityRole(UserType.Teacher.ToString()));
				}
				if (model.UserType.ToString().ToLower() == UserType.Administrator.ToString().ToLower())
				{
					await _userManager.AddToRoleAsync(newUser, UserType.Administrator.ToString());
				}
				if (model.UserType.ToString().ToLower() == UserType.Student.ToString().ToLower())
				{
					await _userManager.AddToRoleAsync(newUser, UserType.Student.ToString());
				}
				else if (model.UserType.ToString().ToLower() == UserType.Teacher.ToString().ToLower())
				{
					await _userManager.AddToRoleAsync(newUser, UserType.Teacher.ToString());
				}
				_response.StatusCode = System.Net.HttpStatusCode.Created;
				_response.IsSuccess = true;
				return _response;
			}
			foreach (var error in result.Errors)
			{
				_response.ErrorMessages.Add(error.ToString());
			}
			return _response;

		}

		public async Task<ApiResponse> GetUserById(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Kullanıcı bulunamadı.");
				return _response;
			}

			var roles = await _userManager.GetRolesAsync(user);
			var role = roles.FirstOrDefault() ?? "Unknown";

			var profile = new
			{
				user.FirstName,
				user.LastName,
				user.Email,
				user.PhoneNumber,
				user.Image,
				user.UserName,
				Role = role
			};

			_response.IsSuccess = true;
			_response.Result = profile;
			_response.StatusCode = System.Net.HttpStatusCode.OK;
			return _response;
		}
	}
}
