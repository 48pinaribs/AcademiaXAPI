using AcademiaX_Business.Abstraction;
using AcademiaX_Core.Models;
using AcademiaX_Business.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Business.Concrete;
using Microsoft.AspNetCore.Authorization;

namespace AcademiaX.API.Controllers
{
	[ApiController]
	[Route("api/student")]
	public class StudentController : ControllerBase
	{
		private readonly IStudentService _studentService;

		public StudentController(IStudentService studentService)
		{
			_studentService = studentService;
		}


		// ✅ 1. Tüm öğrencileri getir
		[HttpGet("all")]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllStudents()
		{
			var response = await _studentService.GetAllStudents();
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 2. ID ile öğrenci getir
		[HttpGet("{studentId}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetStudentById(string studentId)
		{
			var response = await _studentService.GetStudentById(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/profile/{userId}
		[HttpGet("profile/{userId}")]
		public async Task<IActionResult> GetProfile(string userId)
		{
			var response = await _studentService.GetProfile(userId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/courses/{studentId}
		[HttpGet("courses/{studentId}")]
		public async Task<IActionResult> GetEnrolledCourses(string studentId)
		{
			var response = await _studentService.GetEnrolledCourses(studentId);
			return StatusCode((int)response.StatusCode, response);
		}


		// GET: api/student/grades/{studentId}
		[HttpGet("grades/{studentId}")]
		public async Task<IActionResult> GetGrades(string studentId)
		{
			var response = await _studentService.GetGrades(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// POST: api/student/enroll
		[HttpPost("enroll")]
		public async Task<IActionResult> EnrollCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			var response = await _studentService.EnrollCourse(model.StudentId, model.CourseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// DELETE: api/student/drop
		[HttpPost("drop")]
		public async Task<IActionResult> DropCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			var response = await _studentService.DropCourse(model.StudentId, model.CourseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/attendance/{studentId}
		[HttpGet("attendance/{studentId}")]
		public async Task<IActionResult> GetAttendance(string studentId)
		{
			var response = await _studentService.GetAttendance(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// POST: api/student/message/send
		[HttpPost("message/send")]
		public async Task<IActionResult> SendMessageToAdvisor([FromBody] SendMessageDTO model)
		{
			var response = await _studentService.SendMessageToAdvisor(model.StudentId, model.Content);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
