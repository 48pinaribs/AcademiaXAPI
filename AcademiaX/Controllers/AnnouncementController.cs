using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX.Controllers
{
	[ApiController]
	[Route("api/announcement")]
	[Authorize]
	public class AnnouncementController : ApiControllerBase
	{
		private readonly IAnnouncementService _announcementService;

		public AnnouncementController(IAnnouncementService announcementService)
		{
			_announcementService = announcementService;
		}

		// GET: api/announcement/all — giriş yapmış her rol okuyabilir.
		[HttpGet("all")]
		public async Task<IActionResult> GetAll()
		{
			var response = await _announcementService.GetAll();
			return StatusCode((int)response.StatusCode, response);
		}

		// POST: api/announcement/create — sadece Teacher/Administrator.
		[HttpPost("create")]
		[Authorize(Roles = "Teacher,Administrator")]
		public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequestDTO model)
		{
			var response = await _announcementService.Create(model, CurrentUserId);
			return StatusCode((int)response.StatusCode, response);
		}

		// DELETE: api/announcement/delete/{id} — yazan kişi ya da Administrator.
		[HttpDelete("delete/{id}")]
		[Authorize(Roles = "Teacher,Administrator")]
		public async Task<IActionResult> Delete(int id)
		{
			var response = await _announcementService.Delete(id, CurrentUserId, IsAdministrator);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
