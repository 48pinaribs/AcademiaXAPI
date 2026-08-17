using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos.Courses
{
	public class CreateCourseRequestDTO
	{
		[Required]
		[StringLength(200, MinimumLength = 2)]
		public string Name { get; set; }

		[Required]
		[StringLength(20, MinimumLength = 2)]
		public string Code { get; set; }

		[StringLength(2000)]
		public string Description { get; set; }

		[Range(1, 30)]
		public int Credits { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "DepartmentId is required.")]
		public int DepartmentId { get; set; }

		[Range(1, int.MaxValue, ErrorMessage = "SemesterId is required.")]
		public int SemesterId { get; set; }

		[Required]
		public string TeacherId { get; set; }
	}
}
