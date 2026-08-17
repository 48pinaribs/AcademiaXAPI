using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Business.Dtos.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX.Controllers;

[ApiController]
[Route("api/teacher")]
[Authorize]
public class TeacherController : ApiControllerBase
{
	private readonly ITeacherService _teacherService;

	public TeacherController(ITeacherService teacherService)
	{
		_teacherService = teacherService;
	}

	// ✅ 1. Tüm öğretmenleri getir — PII içerir, sadece yönetim görebilir.
	[HttpGet("all")]
	[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> GetAllTeachers()
	{
		var response = await _teacherService.GetAllTeachers();
		return StatusCode((int)response.StatusCode, response);
	}

	// ✅ 2. ID ile öğretmen getir
	[HttpGet("{teacherId}")]
	[Authorize(Roles = "Administrator")]
	public async Task<IActionResult> GetTeacherById(string teacherId)
	{
		var response = await _teacherService.GetTeacherById(teacherId);
		return StatusCode((int)response.StatusCode, response);
	}

	[HttpGet("profile/{id}")]
	public async Task<IActionResult> GetTeacherProfile(string id)
	{
		if (!CanAccessOwnResource(id))
		{
			return Forbid();
		}

		var teacherProfileDto = new TeacherProfileDTO { Id = id };
		var response = await _teacherService.GetTeacherProfile(teacherProfileDto);
		return StatusCode((int)response.StatusCode, response);
	}

	[HttpGet("courses/{teacherId}")]
	public async Task<IActionResult> GetCoursesByTeacher(string teacherId)
	{
		if (!CanAccessOwnResource(teacherId))
		{
			return Forbid();
		}

		var teacherCoursesDto = new TeacherCoursesDTO { TeacherId = teacherId };
		var response = await _teacherService.GetCoursesByTeacher(teacherCoursesDto);
		return StatusCode((int)response.StatusCode, response);
	}

	[HttpPut("update-profile")]
	[Authorize(Roles = "Teacher,Administrator")]
	public async Task<IActionResult> UpdateTeacherProfile([FromBody] UpdateProfileRequestDTO model)
	{
		if (!CanAccessOwnResource(model.Id))
		{
			return Forbid();
		}

		var response = await _teacherService.UpdateTeacherProfile(model);
		return StatusCode((int)response.StatusCode, response);
	}

	// Bir öğrenciyi derse atamak, öğrencinin danışmanı/dersin öğretmeni olan Teacher ya da Administrator'a özeldir.
	[HttpPost("assign-student")]
	[Authorize(Roles = "Teacher,Administrator")]
	public async Task<IActionResult> AssignStudentToCourse([FromBody] EnrollInCourseRequestDTO model)
	{
		var response = await _teacherService.AssignStudentToCourse(model);
		return StatusCode((int)response.StatusCode, response);
	}
}
