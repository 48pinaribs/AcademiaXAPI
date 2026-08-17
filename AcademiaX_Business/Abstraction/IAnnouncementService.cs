using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface IAnnouncementService
	{
		/// Herkes (giriş yapmış her rol) okuyabilir — en yeniden eskiye.
		Task<ApiResponse> GetAll();

		/// Sadece Teacher/Administrator oluşturabilir.
		Task<ApiResponse> Create(CreateAnnouncementRequestDTO model, string authorId);

		/// Yazan kişi ya da Administrator silebilir.
		Task<ApiResponse> Delete(int id, string requestingUserId, bool isAdmin);
	}
}
