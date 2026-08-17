using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class CreateAnnouncementRequestDTO
	{
		[Required]
		[StringLength(200)]
		public string Title { get; set; }

		[Required]
		[StringLength(4000)]
		public string Content { get; set; }
	}
}
