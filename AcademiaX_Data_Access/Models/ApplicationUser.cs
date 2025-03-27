using AcademiaX_Data_Access.Domain;
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
		public string FirstName { get; set; }  // Öğrencinin adı  
		public string LastName { get; set; }   // Öğrencinin soyadı
		public string? ProfilePicture { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Address { get; set; }  // Öğrencinin adresi  
		public DateTime RegistrationDate { get; set; }  // Kayıt tarihi  
		public ICollection<Announcement> Announcements { get; set; }
		public ICollection<Attendance> Attendances { get; set; }
		public ICollection<Course> Courses { get; set; }
		public ICollection<Grade> Grades { get; set; }

		[NotMapped]
		public ICollection<Message> Messages { get; set; }

	}
}
