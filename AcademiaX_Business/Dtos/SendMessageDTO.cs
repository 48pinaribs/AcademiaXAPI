using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class SendMessageDTO
	{
		[Required]
		public string StudentId { get; set; }

		[Required]
		[StringLength(2000, MinimumLength = 1)]
		public string Content { get; set; }
	}
}
