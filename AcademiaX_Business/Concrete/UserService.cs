using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Concrete
{
	public class UserService : IUserService
	{
		public UserService()
		{
		}

		public Task<ApiResponse> Login(LoginRequestDTO model)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponse> Register(RegisterRequestDTO model)
		{
			throw new NotImplementedException();
		}
	}
}
