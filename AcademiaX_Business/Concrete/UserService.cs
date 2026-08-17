using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Enums;
using AcademiaX_Data_Access.Models;
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
		private readonly ApiResponse _response;
		private readonly UserManager<ApplicationUser> _userManager;
		private string secretKey;

		public UserService(UserManager<ApplicationUser> userManager, ApiResponse response, IConfiguration configuration, ApplicationDbContext context)
		{
			_userManager = userManager;
			_response = response;
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



		
		public async Task<ApiResponse> Register(RegisterRequestDTO model)
		{
			// GÜVENLİK: Bu, herkese açık (anonim) kayıt uç noktasıdır. İstemciden gelen UserType
			// alanına GÜVENİLMEZ — aksi halde herhangi biri kendini Administrator/Teacher yapabilir.
			// Public kayıt her zaman Student rolü verir. Teacher/Administrator hesapları yalnızca
			// CreateStaffUser (Administrator yetkisi gerektirir) ile açılabilir.
			return await CreateUserInternal(model, UserType.Student);
		}

		public async Task<ApiResponse> CreateStaffUser(RegisterRequestDTO model)
		{
			if (!Enum.TryParse<UserType>(model.UserType, true, out var requestedType) ||
				requestedType == UserType.Student)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("UserType 'Teacher' veya 'Administrator' olmalı.");
				return _response;
			}

			return await CreateUserInternal(model, requestedType);
		}

		private async Task<ApiResponse> CreateUserInternal(RegisterRequestDTO model, UserType roleToAssign)
		{
			var userFromDb = _context.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == model.UserName.ToLower());
			if (userFromDb != null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("UserName already exist");
				return _response;
			}

			ApplicationUser newUser = new()
			{
				UserName = model.UserName,
				Email = model.Email,
				UserType = roleToAssign,
				// PasswordHash burada atanmaz: aşağıdaki CreateAsync(user, password) çağrısı
				// şifreyi Identity'nin kendi hash algoritmasıyla güvenli şekilde hash'ler.
				PhoneNumber = model.PhoneNumber,
				FirstName = model.FirstName,
				LastName = model.LastName,
				Image = model.Image,
			};

			var result = await _userManager.CreateAsync(newUser, model.Password);

			if (!result.Succeeded)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				foreach (var error in result.Errors)
				{
					_response.ErrorMessages.Add(error.Description);
				}
				return _response;
			}

			await _userManager.AddToRoleAsync(newUser, roleToAssign.ToString());

			_response.StatusCode = System.Net.HttpStatusCode.Created;
			_response.IsSuccess = true;
			return _response;
		}

		public async Task<ApiResponse> GetUserType(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.NotFound;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Kullanıcı bulunamadı.");
				return _response;
			}

			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
			_response.Result = user.UserType?.ToString();
			return _response;
		}

		public async Task<ApiResponse> UpdateProfile(UpdateProfileRequestDTO model)
		{
			var user = await _userManager.FindByIdAsync(model.Id);
			if (user == null)
			{
				_response.StatusCode = System.Net.HttpStatusCode.NotFound;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Kullanıcı bulunamadı.");
				return _response;
			}

			user.FirstName = model.FirstName;
			user.LastName = model.LastName;
			if (!string.IsNullOrWhiteSpace(model.PhoneNumber)) user.PhoneNumber = model.PhoneNumber;
			if (!string.IsNullOrWhiteSpace(model.Address)) user.Address = model.Address;
			if (!string.IsNullOrWhiteSpace(model.Image)) user.Image = model.Image;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				_response.StatusCode = System.Net.HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				foreach (var error in result.Errors)
				{
					_response.ErrorMessages.Add(error.Description);
				}
				return _response;
			}

			_response.StatusCode = System.Net.HttpStatusCode.OK;
			_response.IsSuccess = true;
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
