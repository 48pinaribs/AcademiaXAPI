using AcademiaX_Data_Access.Domain;
using AcademiaX_Data_Access.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Context
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Grade> Grades { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Attendance> Attendances { get; set; }
		public DbSet<Announcement> Announcements { get; set; }

	
	}
}
