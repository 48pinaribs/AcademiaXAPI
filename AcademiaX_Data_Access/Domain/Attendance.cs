using AcademiaX_Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcademiaX_Data_Access.Enums;

namespace AcademiaX_Data_Access.Domain
{

   public class Attendance
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public int StudentId { get; set; } // Yoklaması alınan öğrenci

		[Required]
		public int CourseId { get; set; } // Yoklamanın ait olduğu ders

		[Required]
		public DateTime AttendanceDate { get; set; } = DateTime.UtcNow; // Yoklamanın tarihi

		[Required]
		public AttendanceStatus Status { get; set; } = AttendanceStatus.Present; // Katılım durumu

		// Foreign Key İlişkileri
		[ForeignKey("StudentId")]
		public ApplicationUser Student { get; set; } = null!;

		[ForeignKey("CourseId")]
		public Course Course { get; set; } = null!;
	}


}
}
