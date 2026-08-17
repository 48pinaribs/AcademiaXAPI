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

		// [Phone] tek başına boş string'i geçersiz sayıyordu (sadece null'u kabul eder) —
		// telefon alanı zorunlu değil ve ProfilePage.jsx boşken "" gönderiyor, bu da tüm
		// profil güncellemesini 400'e düşürüyordu. Boş string'e izin veren desen kullanıyoruz.
		[RegularExpression(@"^$|^[0-9+\-\s()]{7,20}$", ErrorMessage = "Geçerli bir telefon numarası girin.")]
		public string PhoneNumber { get; set; }

		// Öğretim üyesine özel alanlar (opsiyonel — Student/Admin profillerinde kullanılmaz).
		[StringLength(150)]
		public string Branch { get; set; }

		[StringLength(150)]
		public string Title { get; set; }

		[StringLength(150)]
		public string Office { get; set; }

		[StringLength(2000)]
		public string Biography { get; set; }
	}
}
