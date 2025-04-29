using AcademiaX_Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Domain
{
	public class Course
	{
		[Key]
		public int Id { get; set; }

		public string? Title { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }  // Dersin adı					
		public int SemesterId { get; set; } // Yarıyıl ID
		public int DepartmentId { get; set; } // Bölüm ID
		public string Description { get; set; }  // Dersin açıklaması	
		public int Credits { get; set; } // Dersin kredisi


		// ✅ Öğrenci ilişkisi (Many-to-Many)
		public ICollection<ApplicationUser> Students { get; set; } 



		// ✅ Öğretmen ilişkisi (One-to-Many)
		public string TeacherId { get; set; }
		public ApplicationUser Teacher { get; set; }
	}

}
