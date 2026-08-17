using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class UpsertGradeRequestDTO
	{
		[Required]
		public string StudentId { get; set; }

		[Required]
		public int CourseId { get; set; }

		// "Midterm" | "Final" | "Resit" — AcademiaX_Data_Access.Enums.ExamType ile eşleşmeli.
		[Required]
		public string ExamType { get; set; }

		[Range(0, 100)]
		public double Value { get; set; }
	}
}
