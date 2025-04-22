using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Enums;
using AcademiaX_Data_Access.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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

		public Task<ApiResponse> Login(LoginRequestDTO model)
		{
			throw new NotImplementedException();
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
			 
			var newUser = _mapper.Map<ApplicationUser>(model);


			var result = await _userManager.CreateAsync(newUser, model.Password);
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
	}
}
