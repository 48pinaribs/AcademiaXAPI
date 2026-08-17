using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	/// <summary>Admin'in bir öğrenciye danışman (Teacher) atamasını/kaldırmasını sağlar.</summary>
	public class AssignAdvisorRequestDTO
	{
		[Required]
		public string StudentId { get; set; }

		// null gönderilirse danışman ataması kaldırılır.
		public string AdvisorId { get; set; }
	}
}
