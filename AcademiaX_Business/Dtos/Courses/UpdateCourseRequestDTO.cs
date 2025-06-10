using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Courses
{
	public class UpdateCourseRequestDTO
	{

		public int CourseId { get; set; }  // Güncellenecek kursun ID'si

		public string Name { get; set; }  // Kursun yeni başlığı

		public string Description { get; set; }  // Kursun yeni açıklaması (opsiyonel olabilir)

	    public string Code { get; set; }

		public int Credits { get; set; }  // Kursun kredi sayısı

		public int DepartmentId { get; set; }  // Hangi bölümde olduğu

		public int SemesterId { get; set; }  // Hangi dönemde olduğu

		public string TeacherId { get; set; }  // Hangi öğretmen verecek
	}
}
