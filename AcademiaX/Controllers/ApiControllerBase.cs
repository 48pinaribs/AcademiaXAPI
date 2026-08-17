using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AcademiaX.Controllers
{
	/// <summary>
	/// IDOR (Insecure Direct Object Reference) önlemi için ortak yardımcılar.
	/// URL/body'den gelen bir kullanıcı ID'sine erişilmeden önce, çağıranın o kaynağın
	/// sahibi olup olmadığı ya da yeterli role sahip olup olmadığı burada kontrol edilir.
	/// </summary>
	public abstract class ApiControllerBase : ControllerBase
	{
		protected string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

		protected string CurrentUserRole => User.FindFirstValue(ClaimTypes.Role);

		protected bool IsAdministrator => CurrentUserRole == "Administrator";

		protected bool IsTeacher => CurrentUserRole == "Teacher";

		/// <summary>Çağıran, hedeflenen kullanıcının kendisi mi ya da Administrator mı?</summary>
		protected bool CanAccessOwnResource(string targetUserId) =>
			IsAdministrator || (!string.IsNullOrEmpty(CurrentUserId) && CurrentUserId == targetUserId);

		/// <summary>Çağıran, hedeflenen kullanıcının kendisi mi, Administrator mı ya da Teacher mı?
		/// Not/yoklama gibi bir öğretmenin de görebilmesi gereken veriler için kullanılır.</summary>
		protected bool CanAccessStudentResource(string targetStudentId) =>
			IsAdministrator || IsTeacher || (!string.IsNullOrEmpty(CurrentUserId) && CurrentUserId == targetStudentId);
	}
}
