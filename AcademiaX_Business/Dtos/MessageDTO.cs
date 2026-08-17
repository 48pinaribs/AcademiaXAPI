using System;

namespace AcademiaX_Business.Dtos
{
	/// <summary>Bir mesaj kutusu (inbox) satırı — gönderenin adı çözümlenmiş halde.</summary>
	public class MessageDTO
	{
		public int Id { get; set; }
		public string SenderId { get; set; }
		public string SenderName { get; set; }
		public string Content { get; set; }
		public DateTime SentAt { get; set; }
		public bool IsRead { get; set; }
	}
}
