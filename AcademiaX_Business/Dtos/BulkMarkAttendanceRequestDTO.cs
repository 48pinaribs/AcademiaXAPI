using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class BulkMarkAttendanceRequestDTO
	{
		[Required]
		public int CourseId { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		[MinLength(1, ErrorMessage = "En az bir öğrenci için yoklama girilmelidir.")]
		public List<AttendanceRecordDTO> Records { get; set; }
	}
}
