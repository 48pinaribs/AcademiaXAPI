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
			public string Name { get; set; }  // Dersin adı  
			public string Description { get; set; }  // Dersin açıklaması	
		    public int Credit { get; set; } // Dersin kredisi
		    public int TeacherId { get; set; }  // Öğretmen bilgisi  
			public ApplicationUser Teacher { get; set; }
		}

}
