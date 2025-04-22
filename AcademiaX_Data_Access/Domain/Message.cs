using AcademiaX_Data_Access.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Domain
{
public class Message
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public string SenderId { get; set; }

		[Required]
		public string ReceiverId { get; set; }

		[Required]
		[MaxLength(1000)]  // Maksimum mesaj uzunluğu belirleyelim
		public string Content { get; set; } = string.Empty;

		[Required]
		public DateTime SentAt { get; set; } = DateTime.UtcNow; // Varsayılan gönderim zamanı

		public bool IsRead { get; set; } = false;  // Varsayılan olarak okunmamış

		// Foreign Keys
		[ForeignKey("SenderId")]
		public ApplicationUser Sender { get; set; } = null!;  // Nullable olmasın

		[ForeignKey("ReceiverId")]
		public ApplicationUser Receiver { get; set; } = null!;  // Nullable olmasın
	}

}

