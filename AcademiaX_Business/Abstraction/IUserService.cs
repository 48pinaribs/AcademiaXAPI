using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface IUserService
	{
		Task<ApiResponse> Register(RegisterRequestDTO model);
		Task<ApiResponse> Login(LoginRequestDTO model);
	}
}
