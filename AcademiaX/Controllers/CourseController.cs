using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AcademiaX.Controllers
{
	[ApiController]
	[Route("api/course")]
	[Authorize]
	public class CourseController : ApiControllerBase
	{
		private readonly ICourseService _courseService;

		public CourseController(ICourseService courseService)
		{
			_courseService = courseService;
		}

		// ✅ 1. Kurs oluştur
		[HttpPost("create")]
		[Authorize(Roles = "Administrator")] // Sadece Admin kurs ekleyebilir
		public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDTO model)
		{
			var response = await _courseService.CreateCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 2. Kurs güncelle
		[HttpPut("update")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseRequestDTO model)
		{
			var response = await _courseService.UpdateCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 3. Kurs sil
		[HttpDelete("delete/{courseId}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> DeleteCourse(int courseId)
		{
			var response = await _courseService.DeleteCourse(courseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 4. Tüm kursları listele — genel kurs kataloğu, herkese açık kalabilir.
		[HttpGet("all")]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllCourses()
		{
			var response = await _courseService.GetAllCourses();
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 5. ID ile kurs getir — genel kurs kataloğu, herkese açık kalabilir.
		[HttpGet("{courseId}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetCourseById(int courseId)
		{
			var response = await _courseService.GetCourseById(courseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 6. Öğrenciyi kursa kaydet — bir öğrenci sadece kendini kaydedebilir.
		[HttpPost("enroll")]
		[Authorize(Roles = "Student,Administrator")]
		public async Task<IActionResult> EnrollInCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			if (!CanAccessOwnResource(model.StudentId))
			{
				return Forbid();
			}

			var response = await _courseService.EnrollInCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 7. Öğrenciyi kurstan çıkar — bir öğrenci sadece kendini çıkarabilir.
		[HttpPost("unenroll")]
		[Authorize(Roles = "Student,Administrator")]
		public async Task<IActionResult> UnenrollFromCourse([FromBody] UnenrollFromCourseRequestDTO model)
		{
			if (!CanAccessOwnResource(model.StudentId))
			{
				return Forbid();
			}

			var response = await _courseService.UnenrollFromCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 8. Öğrencinin kayıtlı olduğu kurslar
		[HttpGet("enrolled/{userId}")]
		[Authorize]
		public async Task<IActionResult> GetEnrolledCourses(string userId)
		{
			if (!CanAccessOwnResource(userId))
			{
				return Forbid();
			}

			var response = await _courseService.GetEnrolledCourses(userId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 9. Öğrencinin kayıt olabileceği kurslar
		[HttpGet("available/{userId}")]
		[Authorize]
		public async Task<IActionResult> GetAvailableCourses(string userId)
		{
			if (!CanAccessOwnResource(userId))
			{
				return Forbid();
			}

			var response = await _courseService.GetAvailableCourses(userId);
			return StatusCode((int)response.StatusCode, response);
		}

		// Bir dersteki öğrenci listesi PII içerir (isim/e-posta) — sadece Teacher/Administrator görebilir.
		[HttpGet("{courseId}/students")]
		[Authorize(Roles = "Teacher,Administrator")]
		public async Task<IActionResult> GetStudentsByCourse(int courseId)
		{
			var response = await _courseService.GetStudentsInCourse(courseId);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
