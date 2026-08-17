using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class FavoriteStopRequestDTO
	{
		[Required]
		public string StudentId { get; set; }

		// null gönderilirse favori durak kaldırılır.
		public string StopId { get; set; }
	}
}
