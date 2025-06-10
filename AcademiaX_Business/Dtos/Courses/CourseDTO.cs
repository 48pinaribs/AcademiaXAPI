using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Courses
{
	public class CourseDTO
	{

		public int Credits { get; set; }  // Kursun kredi sayısı
		public int DepartmentId { get; set; }  // Hangi bölümde olduğu
		public int SemesterId { get; set; }  // Hangi dönemde olduğu
		public string TeacherId { get; set; }  // Hangi öğretmen verecek
		public int CourseId { get; set; }        // Kurs ID
		public string Name { get; set; }         // Kursun başlığı
	    public string Code { get; set; }         // Kursun kodu
		public string Description { get; set; }   // Kurs açıklaması
		public int TotalStudents { get; set; }    // Öğrenci sayısı
	}
}
