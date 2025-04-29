using AcademiaX_Business.Dtos.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos
{
	public class TeacherCoursesDTO
	{
		public string TeacherId { get; set; }     // Öğretmen ID
		public string TeacherName { get; set; } // Öğretmen adı

		public List<CourseDTO> Courses { get; set; } // Öğretmenin verdiği kurslar
	}
}
