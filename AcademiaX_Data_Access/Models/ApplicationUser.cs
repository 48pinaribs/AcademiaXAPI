using AcademiaX_Data_Access.Domain;
using AcademiaX_Data_Access.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class ApplicationUser : IdentityUser
	{
		// ✅ Temel Bilgiler
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateTime? DateOfBirth { get; set; }
		public string? Address { get; set; }
		public DateTime? RegistrationDate { get; set; }
		public string? Image { get; set; } // Profil fotoğrafı dosya adı ya da URL

		// ✅ Kullanıcı tipi: Öğrenci, Öğretmen, Admin (Enum)
		[Column(TypeName = "varchar(20)")]
		public UserType UserType { get; set; }
		public string? Branch { get; set; }
		public string? Title { get; set; }
		public string? Office { get; set; }
		public string? Biography { get; set; }

		public string? Department { get; set; }
		public string? Faculty { get; set; }
		public double? GPA { get; set; }
		public string? AcademicLevel { get; set; }


		// ✅ Duyuru, yoklama, not ilişkileri
		public ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
		public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
		public ICollection<Grade> Grades { get; set; } = new List<Grade>();

		// ✅ Ders kaydı (öğrenci için dersler)
		public ICollection<Course> Courses { get; set; } = new List<Course>();






		// ✅ Danışman ilişkisi (öğrenci → öğretmen)
		public string? AdvisorId { get; set; } // foreign key
		[ForeignKey("AdvisorId")]
		public ApplicationUser? Advisor { get; set; }




		// ✅ Mesajlaşma (gönderilen ve alınan mesajlar)
		//  Veritabanına yansıtılmasın diyorsan:
		[NotMapped]
		public ICollection<Message> SentMessages { get; set; } = new List<Message>();

		[NotMapped]
		public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

	}
}
