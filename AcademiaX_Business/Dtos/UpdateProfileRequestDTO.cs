using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class UpdateProfileRequestDTO
	{
		[Required]
		public string Id { get; set; }

		public string Image { get; set; }

		[StringLength(300)]
		public string Address { get; set; }

		[Required]
		[StringLength(100)]
		public string FirstName { get; set; }

		public string FullName { get; set; }

		[Required]
		[StringLength(100)]
		public string LastName { get; set; }

		[EmailAddress]
		public string Email { get; set; }

		[Phone]
		public string PhoneNumber { get; set; }
	}
}
