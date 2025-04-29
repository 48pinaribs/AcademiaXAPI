using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos
{
	public class LoginRequestDTO
	{
	
			[Required(ErrorMessage = "User ID is required.")]
			public string UserName { get; set; }
			// Öğrenci numarası, öğretmen ID'si ya da admin ID'si

			[Required(ErrorMessage = "Password is required.")]
			[MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
			public string Password { get; set; }

		}
	
}
