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


		//Ders-Öğrenci ilişkisi: Çoktan çoğa → StudentCourses tablosu oluşturulur.
		//Ders-Öğretmen ilişkisi: Bire çok, ama silme işleminde öğretmen silinemez(Restrict).
		//c → Course nesnesi (bir ders)
		//u → ApplicationUser nesnesi(bir öğrenci)
		//OnModelCreating metodu içinde yaptığın şey, Entity Framework Core'da ilişkileri manuel olarak konfigüre etmek.
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Course>()
				.HasMany(c => c.Students)
				.WithMany(u => u.Courses)
				.UsingEntity(j => j.ToTable("StudentCourses"));

			modelBuilder.Entity<Course>()
	       .HasOne(c => c.Teacher)
	       .WithMany() // eğer ApplicationUser tarafında karşılık tanımlamadıysan boş bırak
	       .HasForeignKey(c => c.TeacherId)
	       .OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<ApplicationUser>()
	       .Property(e => e.UserType)
	       .HasConversion<string>();


		}


	}
}
