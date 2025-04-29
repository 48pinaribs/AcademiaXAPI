using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos
{
	public class TeacherProfileDTO
	{
		// Modelde var:
		public string Id { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Image { get; set; }
		public string Branch { get; set; }
		public string Title { get; set; }
		public string Office { get; set; }
		public string Biography { get; set; }

		// Modelde yok, DTO'da hesaplanacak:
		public string FullName { get; set; } // FirstName + LastName birleşimi
		public int TotalStudents { get; set; } // Verdiği derslerdeki toplam öğrenci sayısı
		public int CoursesGivenCount { get; set; } // Verdiği toplam ders sayısı
	}
}
