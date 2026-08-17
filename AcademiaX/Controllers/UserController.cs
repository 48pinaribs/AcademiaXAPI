using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class UserController : ApiControllerBase
	{
		/* ASP.NET Core uygulamalarında Dependency Injection (Bağımlılık Enjeksiyonu) kullanılarak bir servisin bir kontrolcüye (controller) enjekte edilmesini gösteriyor */
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("Register")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateUser([FromBody] RegisterRequestDTO model)
		{
			var response = await _userService.Register(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}

		// Yalnızca Administrator'ın Teacher/Administrator hesabı açabildiği uç nokta.
		// Public Register her zaman Student rolü verir (bkz. UserService.Register).
		[HttpPost("CreateStaffUser")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> CreateStaffUser([FromBody] RegisterRequestDTO model)
		{
			var response = await _userService.CreateStaffUser(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}

		[HttpPost("Login")]
		[AllowAnonymous]
		public async Task<IActionResult> LoginUser([FromBody] LoginRequestDTO model)
		{
			var response = await _userService.Login(model);
			if (response.IsSuccess)
			{
				return Ok(response);
			}
			return BadRequest(response);
		}

		// Kullanıcı sadece kendi profilini, Administrator ise herkesinkini görebilir.
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUserById(string id)
		{
			if (!CanAccessOwnResource(id))
			{
				return Forbid();
			}

			var result = await _userService.GetUserById(id);
			return StatusCode((int)result.StatusCode, result);
		}

		// Bir kullanıcının Student/Teacher/Administrator olduğunu öğrenmek yönetimsel bir işlemdir
		// (ör. admin panelindeki kullanıcı detay ekranı) — herkese açık olmamalı.
		[HttpGet("GetUserType/{id}")]
		[Authorize(Roles = "Administrator,Teacher")]
		public async Task<IActionResult> GetUserType(string id)
		{
			var result = await _userService.GetUserType(id);
			return StatusCode((int)result.StatusCode, result);
		}

		// Kullanıcı sadece kendi profilini güncelleyebilir, Administrator herkesinkini.
		[HttpPut("update-profile")]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequestDTO model)
		{
			if (!CanAccessOwnResource(model.Id))
			{
				return Forbid();
			}

			var result = await _userService.UpdateProfile(model);
			return StatusCode((int)result.StatusCode, result);
		}
	}
}
