using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface IUserService
	{
		/// <summary>Herkese açık kayıt. Gönderilen UserType'tan bağımsız olarak her zaman Student rolü atanır.</summary>
		Task<ApiResponse> Register(RegisterRequestDTO model);
		Task<ApiResponse> Login(LoginRequestDTO model);
		Task<ApiResponse> GetUserById(string userId);

		/// <summary>Sadece Administrator çağırabilir (controller'da [Authorize(Roles = "Administrator")]).
		/// Teacher veya Administrator rolünde hesap oluşturur.</summary>
		Task<ApiResponse> CreateStaffUser(RegisterRequestDTO model);

		Task<ApiResponse> GetUserType(string userId);

		/// <summary>Kullanıcının kendi profilini güncellemesi için (ad/soyad/telefon/adres/foto).
		/// Rol bazlı ek alanlar (Teacher'ın Branch/Title/Office/Biography'si gibi) kapsam dışı —
		/// onlar için ayrı TeacherController.UpdateTeacherProfile kullanılır.</summary>
		Task<ApiResponse> UpdateProfile(UpdateProfileRequestDTO model);
	}
}
