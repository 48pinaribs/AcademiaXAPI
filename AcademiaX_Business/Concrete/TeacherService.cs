using AcademiaX_Core.Models;
using AcademiaX_Business.Abstraction;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AcademiaX_Business.Dtos;
using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Data_Access.Enums;

namespace AcademiaX_Business.Concrete;

public class TeacherService : ITeacherService
{
	private readonly ApplicationDbContext _context;

	public TeacherService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task<ApiResponse> GetAllTeachers()
	{
		var response = new ApiResponse();
		var teachers = await _context.ApplicationUsers
			.Where(u => u.UserType == UserType.Teacher)
			.Select(PersonProjections.ToPersonDto)
			.ToListAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = teachers;
		return response;
	}

	public async Task<ApiResponse> GetTeacherById(string teacherId)
	{
		var response = new ApiResponse();
		var teacher = await _context.ApplicationUsers
			.Where(u => u.UserType == UserType.Teacher && u.Id == teacherId)
			.Select(PersonProjections.ToPersonDto)
			.FirstOrDefaultAsync();
		if (teacher == null)
		{
			response.StatusCode = HttpStatusCode.NotFound;
			response.IsSuccess = false;
			response.ErrorMessages.Add("Student not found.");
			return response;
		}
		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = teacher;
		return response;
	}


	public async Task<ApiResponse> GetCoursesByTeacher(TeacherCoursesDTO model)
	{
		var response = new ApiResponse();

		// Ham Course entity'si değil CourseDTO döndürülüyor: Course.Teacher/Students navigation
		// property'leri ileride bir Include ile yüklenirse ApplicationUser'ın (Identity) PasswordHash
		// gibi hassas alanları JSON'a sızdırma riski var — DTO projeksiyonu bunu yapısal olarak engeller.
		var courses = await _context.Courses
			.Where(c => c.TeacherId == model.TeacherId)
			.Select(c => new CourseDTO
			{
				CourseId = c.Id,
				Name = c.Name,
				Code = c.Code,
				Description = c.Description,
				Credits = c.Credits,
				DepartmentId = c.DepartmentId,
				SemesterId = c.SemesterId,
				TeacherId = c.TeacherId,
				TotalStudents = c.Students.Count()
			})
			.ToListAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = courses;

		return response;
	}

	public async Task<ApiResponse> GetTeacherProfile(TeacherProfileDTO model)
	{
		var response = new ApiResponse();

		// Not: TotalStudents/CoursesGivenCount DTO'da tanımlıydı ama hiç hesaplanmıyordu
		// (varsayılan 0 dönüyordu) — UserDetail.jsx bunları gösteriyor, şimdi gerçek değer geliyor.
		// ApplicationUser tarafında "verdiği dersler" için bir navigation property yok
		// (Course.Teacher ilişkisi .WithMany() ile shadow olarak tanımlı), bu yüzden Courses
		// tablosu TeacherId üzerinden ayrıca sorgulanıyor.
		var teacher = await _context.ApplicationUsers
			.Where(u => u.UserType == UserType.Teacher && u.Id == model.Id)
			.Select(u => new TeacherProfileDTO
			{
				Id = u.Id,
				FullName = u.FirstName + " " + u.LastName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				Image = u.Image,
				Branch = u.Branch,
				Title = u.Title,
				Office = u.Office,
				Biography = u.Biography
			})
			.FirstOrDefaultAsync();

		if (teacher != null)
		{
			teacher.CoursesGivenCount = await _context.Courses.CountAsync(c => c.TeacherId == model.Id);
			teacher.TotalStudents = await _context.Courses
				.Where(c => c.TeacherId == model.Id)
				.SelectMany(c => c.Students)
				.Select(s => s.Id)
				.Distinct()
				.CountAsync();
		}

		//var teacher = _context.ApplicationUsers.FirstOrDefault(u => u.Id == model.Id);

		if (teacher == null)
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.NotFound;
			response.ErrorMessages.Add("Öğretmen bulunamadı.");
			return response;
		}

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = teacher;

		return response;
	}

	public async Task<ApiResponse> UpdateTeacherProfile(UpdateProfileRequestDTO model)
	{
		var response = new ApiResponse();

		var teacher = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == model.Id);

		if (teacher == null)
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.NotFound;
			response.ErrorMessages.Add("Öğretmen bulunamadı.");
			return response;
		}

		teacher.FirstName = model.FirstName;
		teacher.LastName = model.LastName;
		teacher.Address = model.Address;
		teacher.PhoneNumber = model.PhoneNumber;
		teacher.Image = model.Image;

		await _context.SaveChangesAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = "Profil başarıyla güncellendi.";

		return response;
	}

	public async Task<ApiResponse> AssignStudentToCourse(EnrollInCourseRequestDTO model)
	{
		var response = new ApiResponse();

		// Ortak "öğrenciyi derse ekle" mantığı: bkz. CourseEnrollmentHelper (önceden bu kod
		// CourseService/StudentService/TeacherService'te üç kez tekrarlanıyordu).
		var result = await CourseEnrollmentHelper.EnrollStudentAsync(_context, model.CourseId, model.StudentId, requireStudentRole: false);

		if (!result.Success)
		{
			response.IsSuccess = false;
			response.StatusCode = result.NotFound ? HttpStatusCode.NotFound : HttpStatusCode.BadRequest;
			response.ErrorMessages.Add(result.ErrorMessage);
			return response;
		}

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = "Öğrenci derse başarıyla eklendi.";

		return response;
	}
}
