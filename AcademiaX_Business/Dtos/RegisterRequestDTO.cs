using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class RegisterRequestDTO
	{
		[Required(ErrorMessage = "Username is required.")]
		[StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
		public string UserName { get; set; }

		[Required(ErrorMessage = "Password is required.")]
		[MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
		public string Password { get; set; }

		// Public Register akışında bu alan kullanılmaz (her zaman Student atanır);
		// yalnızca admin'in çağırabildiği CreateStaffUser için anlamlıdır (Teacher/Administrator).
		public string UserType { get; set; }

		[Required(ErrorMessage = "Email is required.")]
		[EmailAddress(ErrorMessage = "A valid email address is required.")]
		public string Email { get; set; }

		public string Image { get; set; }

		[Required(ErrorMessage = "First name is required.")]
		[StringLength(100)]
		public string FirstName { get; set; }

		[Required(ErrorMessage = "Last name is required.")]
		[StringLength(100)]
		public string LastName { get; set; }

		[Phone(ErrorMessage = "A valid phone number is required.")]
		public string PhoneNumber { get; set; }
	}
}
