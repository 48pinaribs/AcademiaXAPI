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
	public class Grade
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string StudentId { get; set; } // Notu alan öğrenci

		[Required]
		public int CourseId { get; set; } // Notun verildiği ders

		public ExamType ExamType { get; set; }  // Vize / Final / Bütünleme
		public double Value { get; set; }       // Not değeri (0-100)

		[Required]
		public double TotalGrade { get; set; } // Toplam not (Vize ve Final notlarından hesaplanabilir)

		// Foreign Key İlişkileri
		[ForeignKey("StudentId")]
		public ApplicationUser Student { get; set; } = null!; // Öğrenci

		[ForeignKey("CourseId")]
		public Course Course { get; set; } = null!; // Ders
	}
}
