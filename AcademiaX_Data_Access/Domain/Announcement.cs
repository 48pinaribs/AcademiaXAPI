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

		// Not: eskiden burası "int UserId" idi ama ApplicationUser.Id (IdentityUser) string —
		// tip uyuşmazlığı yüzünden EF Core bu ilişkiyi gerçek FK olarak kuramıyor, sessizce
		// "UserId1" adında ayrı bir shadow property/kolon üretiyordu (bkz. startup log uyarısı).
		// String'e çevrilince gerçek bir FK ilişkisi kuruluyor.
		public string UserId { get; set; }
		public ApplicationUser User { get; set; }

	}
}
