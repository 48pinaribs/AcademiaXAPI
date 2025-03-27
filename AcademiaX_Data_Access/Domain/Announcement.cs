using AcademiaX_Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Domain
{
	public class Announcement
	{
		[Key]
		public int Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public DateTime DatePosted { get; set; }
		public int UserId { get; set; }  
		public ApplicationUser User { get; set; }

	}
}
