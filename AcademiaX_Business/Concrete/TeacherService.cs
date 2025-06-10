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
	.Select(u => new PersonDTO
	{
		Id = u.Id,
		FullName = u.FirstName + " " + u.LastName,
		Email = u.Email,
		PhoneNumber = u.PhoneNumber,
		Image = u.Image
	})
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
			.Select(u => new PersonDTO
			{
				Id = u.Id,
				FullName = u.FirstName + " " + u.LastName,
				Email = u.Email,
				PhoneNumber = u.PhoneNumber,
				Image = u.Image
			})
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


	public Task<ApiResponse> GetCoursesByTeacher(TeacherCoursesDTO model)
	{
		var response = new ApiResponse();

		List<AcademiaX_Data_Access.Domain.Course> courses = _context.Courses
			.Where(c => c.TeacherId == model.TeacherId)
			.ToList();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = courses;

		return Task.FromResult(response);
	}

	public async Task<ApiResponse> GetTeacherProfile(TeacherProfileDTO model)
	{
		var response = new ApiResponse();

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

	public Task<ApiResponse> UpdateTeacherProfile(UpdateProfileRequestDTO model)
	{
		var response = new ApiResponse();

		var teacher = _context.ApplicationUsers.FirstOrDefault(u => u.Id == model.Id);

		if (teacher == null)
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.NotFound;
			response.ErrorMessages.Add("Öğretmen bulunamadı.");
			return Task.FromResult(response);
		}

		teacher.FirstName = model.FirstName;
		teacher.LastName = model.LastName;
		teacher.Address = model.Address;
		teacher.PhoneNumber = model.PhoneNumber;
		teacher.Image = model.Image;

		_context.SaveChanges();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = "Profil başarıyla güncellendi.";

		return Task.FromResult(response);
	}

	public Task<ApiResponse> AssignStudentToCourse(EnrollInCourseRequestDTO model)
	{
		var response = new ApiResponse();

		var course = _context.Courses
			.Include(c => c.Students)
			.FirstOrDefault(c => c.Id == model.CourseId);

		if (course == null)
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.NotFound;
			response.ErrorMessages.Add("Ders bulunamadı.");
			return Task.FromResult(response);
		}

		var student = _context.ApplicationUsers.FirstOrDefault(u => u.Id == model.StudentId);
		if (student == null)
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.NotFound;
			response.ErrorMessages.Add("Öğrenci bulunamadı.");
			return Task.FromResult(response);
		}

		// Öğrenci zaten ekli mi?
		if (course.Students.Any(s => s.Id == model.StudentId))
		{
			response.IsSuccess = false;
			response.StatusCode = HttpStatusCode.BadRequest;
			response.ErrorMessages.Add("Öğrenci zaten bu derse kayıtlı.");
			return Task.FromResult(response);
		}

		course.Students.Add(student);
		_context.SaveChanges();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = "Öğrenci derse başarıyla eklendi.";

		return Task.FromResult(response);
	}
}
