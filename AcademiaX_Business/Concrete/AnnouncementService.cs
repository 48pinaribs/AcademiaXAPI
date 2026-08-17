using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace AcademiaX_Business.Concrete
{
	public class AnnouncementService : IAnnouncementService
	{
		private readonly ApplicationDbContext _context;

		public AnnouncementService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ApiResponse> GetAll()
		{
			var response = new ApiResponse();

			var announcements = await _context.Announcements
				.Include(a => a.User)
				.OrderByDescending(a => a.DatePosted)
				.Select(a => new AnnouncementDTO
				{
					Id = a.Id,
					Title = a.Title,
					Content = a.Content,
					DatePosted = a.DatePosted,
					AuthorId = a.UserId,
					AuthorName = a.User.FirstName + " " + a.User.LastName,
				})
				.ToListAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = announcements;
			return response;
		}

		public async Task<ApiResponse> Create(CreateAnnouncementRequestDTO model, string authorId)
		{
			var response = new ApiResponse();

			var announcement = new Announcement
			{
				Title = model.Title,
				Content = model.Content,
				DatePosted = DateTime.UtcNow,
				UserId = authorId,
			};

			_context.Announcements.Add(announcement);
			await _context.SaveChangesAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = "Duyuru yayınlandı.";
			return response;
		}

		public async Task<ApiResponse> Delete(int id, string requestingUserId, bool isAdmin)
		{
			var response = new ApiResponse();

			var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
			if (announcement == null)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Duyuru bulunamadı.");
				return response;
			}

			if (!isAdmin && announcement.UserId != requestingUserId)
			{
				response.StatusCode = HttpStatusCode.Forbidden;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Sadece kendi duyurunuzu silebilirsiniz.");
				return response;
			}

			_context.Announcements.Remove(announcement);
			await _context.SaveChangesAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = "Duyuru silindi.";
			return response;
		}
	}
}
