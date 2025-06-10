using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Concrete;
using AcademiaX_Business.Dtos;
using AcademiaX_Business.Dtos.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX_API.Controllers;

[ApiController]
[Route("api/teacher")]
public class TeacherController : ControllerBase
{
	private readonly ITeacherService _teacherService;

	public TeacherController(ITeacherService teacherService)
	{
		_teacherService = teacherService;
	}

	// ✅ 1. Tüm öğretmenleri getir
	[HttpGet("all")]
	[AllowAnonymous]
	public async Task<IActionResult> GetAllTeachers()
	{
		var response = await _teacherService.GetAllTeachers();
		return StatusCode((int)response.StatusCode, response);
	}

	// ✅ 2. ID ile öğretmen getir
	[HttpGet("{teacherId}")]
	[AllowAnonymous]
	public async Task<IActionResult> GetTeacherById(string teacherId)
	{
		var response = await _teacherService.GetTeacherById(teacherId);
		return StatusCode((int)response.StatusCode, response);
	}

	[HttpGet("profile/{id}")]
	public IActionResult GetTeacherProfile(string id)
	{
		var teacherProfileDto = new TeacherProfileDTO { Id = id };
		var response = _teacherService.GetTeacherProfile(teacherProfileDto);
		return StatusCode((int)response.Result.StatusCode, response.Result);
	}

	[HttpGet("courses/{teacherId}")]
	public IActionResult GetCoursesByTeacher(string teacherId)
	{
		var teacherCoursesDto = new TeacherCoursesDTO { TeacherId = teacherId }; // Corrected property name
		var response = _teacherService.GetCoursesByTeacher(teacherCoursesDto);
		return StatusCode((int)response.Result.StatusCode, response.Result);
	}

	[HttpPut("update-profile")]
	public IActionResult UpdateTeacherProfile([FromBody] UpdateProfileRequestDTO model)
	{
		var response = _teacherService.UpdateTeacherProfile(model);
		return StatusCode((int)response.Result.StatusCode, response.Result);
	}

	[HttpPost("assign-student")]
	public IActionResult AssignStudentToCourse([FromBody] EnrollInCourseRequestDTO model)
	{
		var response = _teacherService.AssignStudentToCourse(model);
		return StatusCode((int)response.Result.StatusCode, response.Result);
	}
}
