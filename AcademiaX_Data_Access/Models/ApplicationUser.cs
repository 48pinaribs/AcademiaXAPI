using AcademiaX_Data_Access.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string? FullName { get; set; }
		public string? ProfilePicture { get; set; }
		public DateTime DateOfBirth { get; set; }
		public ICollection<Announcement> Announcements { get; set; }
		public ICollection<Attendance> Attendances { get; set; }
		public ICollection<Course> Courses { get; set; }
		public ICollection<Grade> Grades { get; set; }
		public ICollection<Message> Messages { get; set; }

	}
}
