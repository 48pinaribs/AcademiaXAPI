using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AcademiaX_Business.Dtos.Courses;
using Microsoft.AspNetCore.Authorization;

namespace AcademiaX.Controllers
{
	[ApiController]
	[Route("api/student")]
	[Authorize]
	public class StudentController : ApiControllerBase
	{
		private readonly IStudentService _studentService;

		public StudentController(IStudentService studentService)
		{
			_studentService = studentService;
		}

		// ✅ 1. Tüm öğrencileri getir — PII (e-posta/telefon) içerir, sadece yönetim görebilir.
		[HttpGet("all")]
		[Authorize(Roles = "Administrator,Teacher")]
		public async Task<IActionResult> GetAllStudents()
		{
			var response = await _studentService.GetAllStudents();
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 2. ID ile öğrenci getir
		[HttpGet("{studentId}")]
		[Authorize(Roles = "Administrator,Teacher")]
		public async Task<IActionResult> GetStudentById(string studentId)
		{
			var response = await _studentService.GetStudentById(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/profile/{userId}
		// Not: CanAccessStudentResource kullanılıyor (CanAccessOwnResource değil) — Teacher'ın da
		// /userdetail sayfasında öğrenci profili görebilmesi gerekiyor (bkz. UserDetail.jsx).
		[HttpGet("profile/{userId}")]
		public async Task<IActionResult> GetProfile(string userId)
		{
			if (!CanAccessStudentResource(userId))
			{
				return Forbid();
			}

			var response = await _studentService.GetProfile(userId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/courses/{studentId}
		[HttpGet("courses/{studentId}")]
		public async Task<IActionResult> GetEnrolledCourses(string studentId)
		{
			if (!CanAccessStudentResource(studentId))
			{
				return Forbid();
			}

			var response = await _studentService.GetEnrolledCourses(studentId);
			return StatusCode((int)response.StatusCode, response);
		}


		// GET: api/student/grades/{studentId}
		[HttpGet("grades/{studentId}")]
		public async Task<IActionResult> GetGrades(string studentId)
		{
			if (!CanAccessStudentResource(studentId))
			{
				return Forbid();
			}

			var response = await _studentService.GetGrades(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// POST: api/student/enroll
		[HttpPost("enroll")]
		[Authorize(Roles = "Student,Administrator")]
		public async Task<IActionResult> EnrollCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			if (!CanAccessOwnResource(model.StudentId))
			{
				return Forbid();
			}

			var response = await _studentService.EnrollCourse(model.StudentId, model.CourseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// DELETE: api/student/drop
		[HttpPost("drop")]
		[Authorize(Roles = "Student,Administrator")]
		public async Task<IActionResult> DropCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			if (!CanAccessOwnResource(model.StudentId))
			{
				return Forbid();
			}

			var response = await _studentService.DropCourse(model.StudentId, model.CourseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// GET: api/student/attendance/{studentId}
		[HttpGet("attendance/{studentId}")]
		public async Task<IActionResult> GetAttendance(string studentId)
		{
			if (!CanAccessStudentResource(studentId))
			{
				return Forbid();
			}

			var response = await _studentService.GetAttendance(studentId);
			return StatusCode((int)response.StatusCode, response);
		}

		// POST: api/student/message/send
		[HttpPost("message/send")]
		[Authorize(Roles = "Student,Administrator")]
		public async Task<IActionResult> SendMessageToAdvisor([FromBody] SendMessageDTO model)
		{
			if (!CanAccessOwnResource(model.StudentId))
			{
				return Forbid();
			}

			var response = await _studentService.SendMessageToAdvisor(model.StudentId, model.Content);
			return StatusCode((int)response.StatusCode, response);
		}

		// PUT: api/student/advisor — bir öğrenciye danışman (Teacher) atar/kaldırır. Sadece Admin.
		[HttpPut("advisor")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> AssignAdvisor([FromBody] AssignAdvisorRequestDTO model)
		{
			var response = await _studentService.AssignAdvisor(model);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
