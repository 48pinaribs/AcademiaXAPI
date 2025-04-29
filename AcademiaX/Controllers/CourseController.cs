using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AcademiaX_API.Controllers
{
	[ApiController]
	[Route("api/course")]
	public class CourseController : ControllerBase
	{
		private readonly ICourseService _courseService;

		public CourseController(ICourseService courseService)
		{
			_courseService = courseService;
		}

		// ✅ 1. Kurs oluştur
		[HttpPost("create")]
		//[Authorize(Roles = "Admin")] // Sadece Admin kurs ekleyebilir
		public async Task<IActionResult> CreateCourse([FromBody] CreateCourseRequestDTO model)
		{
			var response = await _courseService.CreateCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 2. Kurs güncelle
		[HttpPut("update")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateCourse([FromBody] UpdateCourseRequestDTO model)
		{
			var response = await _courseService.UpdateCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 3. Kurs sil
		[HttpDelete("delete/{courseId}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteCourse(int courseId)
		{
			var response = await _courseService.DeleteCourse(courseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 4. Tüm kursları listele
		[HttpGet("all")]
		[AllowAnonymous]
		public async Task<IActionResult> GetAllCourses()
		{
			var response = await _courseService.GetAllCourses();
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 5. ID ile kurs getir
		[HttpGet("{courseId}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetCourseById(int courseId)
		{
			var response = await _courseService.GetCourseById(courseId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 6. Öğrenciyi kursa kaydet
		[HttpPost("enroll")]
		//[Authorize(Roles = "Student,Admin")] // İstersen Student rolü de kaydolabilir
		public async Task<IActionResult> EnrollInCourse([FromBody] EnrollInCourseRequestDTO model)
		{
			var response = await _courseService.EnrollInCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 7. Öğrenciyi kurstan çıkar
		[HttpPost("unenroll")]
		//[Authorize(Roles = "Student,Admin")]
		public async Task<IActionResult> UnenrollFromCourse([FromBody] UnenrollFromCourseRequestDTO model)
		{
			var response = await _courseService.UnenrollFromCourse(model);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 8. Öğrencinin kayıtlı olduğu kurslar
		[HttpGet("enrolled/{userId}")]
		//[Authorize(Roles = "Student,Admin")]
		public async Task<IActionResult> GetEnrolledCourses(string userId)
		{
			var response = await _courseService.GetEnrolledCourses(userId);
			return StatusCode((int)response.StatusCode, response);
		}

		// ✅ 9. Öğrencinin kayıt olabileceği kurslar
		[HttpGet("available/{userId}")]
		//[Authorize(Roles = "Student,Admin")]
		public async Task<IActionResult> GetAvailableCourses(string userId)
		{
			var response = await _courseService.GetAvailableCourses(userId);
			return StatusCode((int)response.StatusCode, response);
		}
	}
}
