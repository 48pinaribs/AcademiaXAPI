using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Courses
{
	public class UnenrollFromCourseRequestDTO
	{
		[Required]
		public string StudentId { get; set; } // Öğrencinin ID'si
		[Required]
		public int CourseId { get; set; } // Kursun ID'si
	}
}
