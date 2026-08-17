using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class AttendanceRecordDTO
	{
		[Required]
		public string StudentId { get; set; }

		// "Present" | "Absent" | "Excused" — AcademiaX_Data_Access.Enums.AttendanceStatus ile eşleşmeli.
		[Required]
		public string Status { get; set; }
	}
}
