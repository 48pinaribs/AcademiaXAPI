using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos.Courses
{
	public class UpdateCourseRequestDTO
	{
		[Range(1, int.MaxValue, ErrorMessage = "CourseId is required.")]
		public int CourseId { get; set; }  // Güncellenecek kursun ID'si

		[Required]
		[StringLength(200, MinimumLength = 2)]
		public string Name { get; set; }  // Kursun yeni başlığı

		[StringLength(2000)]
		public string Description { get; set; }  // Kursun yeni açıklaması (opsiyonel olabilir)

		[Required]
		[StringLength(20, MinimumLength = 2)]
		public string Code { get; set; }

		[Range(1, 30)]
		public int Credits { get; set; }  // Kursun kredi sayısı

		[Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
		public int DepartmentId { get; set; }  // Hangi bölümde olduğu

		[Range(1, int.MaxValue, ErrorMessage = "SemesterId is required.")]
		public int SemesterId { get; set; }  // Hangi dönemde olduğu

		[Required]
		public string TeacherId { get; set; }  // Hangi öğretmen verecek
	}
}
