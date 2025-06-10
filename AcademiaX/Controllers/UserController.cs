using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Data_Access.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcademiaX.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		/* ASP.NET Core uygulamalarında Dependency Injection (Bağımlılık Enjeksiyonu) kullanılarak bir servisin bir kontrolcüye (controller) enjekte edilmesini gösteriyor */
		private readonly IUserService _userService;
		private readonly ApplicationDbContext _context;
		public UserController(IUserService userService ,ApplicationDbContext context)
		{
			_userService = userService;
			_context = context;
		}

		[HttpPost("Register")]
		public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDTO model)
		{
			var response = await _userService.Register(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}


		[HttpPost("Login")]
		public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO model)
		{
			var response = await _userService.Login(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}
		//[Authorize(Roles = "Administrator")] // Admin yetkisi olanlar erişebilir
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(string id)
		{
			var result = await _userService.GetUserById(id);
			return StatusCode((int)result.StatusCode, result);
		}

		[HttpGet("GetUserType/{id}")]
		public IActionResult GetUserType(string id)
		{
			var user = _context.ApplicationUsers.FirstOrDefault(u => u.Id == id);
			if (user == null)
				return NotFound(new { message = "Kullanıcı bulunamadı." });

			return Ok(new { result = user.UserType.ToString() }); // örn: "Teacher" veya "Student"
		}


	}
}
