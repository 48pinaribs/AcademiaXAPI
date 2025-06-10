using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Domain;
using AcademiaX_Data_Access.Enums;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AcademiaX_Business.Concrete
{
	public class StudentService : IStudentService
	{
		private readonly ApplicationDbContext _context;

		public StudentService(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<ApiResponse> GetAllStudents()
		{
			var response = new ApiResponse();
			var students = await _context.ApplicationUsers
	      .Where(u => u.UserType == UserType.Student)
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
			response.Result = students;
			return response;
		}

		public async Task<ApiResponse> GetStudentById(string studentId)
		{
			var response = new ApiResponse();
			var student = await _context.ApplicationUsers
				.Where(u => u.UserType == UserType.Student && u.Id == studentId)
				.Select(u => new PersonDTO
				{
					Id = u.Id,
					FullName = u.FirstName + " " + u.LastName,
					Email = u.Email,
					PhoneNumber = u.PhoneNumber,
					Image = u.Image
				})
				.FirstOrDefaultAsync();
			if (student == null)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Student not found.");
				return response;
			}
			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = student;
			return response;
		}

		public async Task<ApiResponse> GetProfile(string userId)
		{
			var response = new ApiResponse();

			var user = await _context.ApplicationUsers.FindAsync(userId);

			if (user == null || user.UserType != UserType.Student)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Student not found.");
				return response;
			}

			var dto = new
			{
				UserName = user.UserName,
				FullName = user.FirstName + " " + user.LastName,
				Email = user.Email,
				Phone = user.PhoneNumber,
				Image = user.Image,

			};

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = dto;
			return response;
		}

		public async Task<ApiResponse> GetEnrolledCourses(string studentId)
		{
			var response = new ApiResponse();

			// Burada Course -> Students ilişkisi varsa Include kullanılabilir
			var courses = await _context.Courses
				.Where(c => c.Students.Any(s => s.Id == studentId)) // navigation property varsa
				.Select(c => new
				{
					c.Code,
					c.Name,
					InstructorName = c.Teacher.FirstName + " " + c.Teacher.LastName
				})
				.ToListAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = courses;
			return response;
		}


		public async Task<ApiResponse> GetGrades(string studentId)
		{
			var response = new ApiResponse();

			var grades = await _context.Grades
				.Where(g => g.StudentId == studentId)
				.Include(g => g.Course)
				.Select(g => new
				{
					Course = g.Course.Name,
					Grade = g.Value
				})
				.ToListAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = grades;
			return response;
		}

		public async Task<ApiResponse> EnrollCourse(string studentId, int courseId)
		{
			var response = new ApiResponse();

			var course = await _context.Courses.Include(c => c.Students)
				.FirstOrDefaultAsync(c => c.Id == courseId);

			if (course == null)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Course not found.");
				return response;
			}

			var student = await _context.ApplicationUsers.FindAsync(studentId);
			if (student == null || student.UserType != UserType.Student)
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Invalid student.");
				return response;
			}

			course.Students.Add(student);
			await _context.SaveChangesAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = true;
			return response;
		}

		public async Task<ApiResponse> DropCourse(string studentId, int courseId)
		{
			var response = new ApiResponse();

			var course = await _context.Courses.Include(c => c.Students)
				.FirstOrDefaultAsync(c => c.Id == courseId);

			if (course == null)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Course not found.");
				return response;
			}

			var student = course.Students.FirstOrDefault(s => s.Id == studentId);
			if (student == null)
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Student not enrolled in this course.");
				return response;
			}

			course.Students.Remove(student);
			await _context.SaveChangesAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = true;
			return response;
		}

		public async Task<ApiResponse> SendMessageToAdvisor(string studentId, string message)
		{
			var response = new ApiResponse();

			var student = await _context.ApplicationUsers
				.FirstOrDefaultAsync(u => u.Id == studentId && u.UserType == UserType.Student);

			if (student == null)
			{
				response.StatusCode = HttpStatusCode.NotFound;
				response.IsSuccess = false;
				response.ErrorMessages.Add("Student not found.");
				return response;
			}

			// Şimdilik advisorId sabit, ileride eşleştirme yapılabilir
			var newMessage = new Message
			{
				SenderId = studentId,
				ReceiverId = student.AdvisorId, // AdvisorId alanı varsa
				Content = message,
				SentAt = DateTime.UtcNow
			};

			_context.Messages.Add(newMessage);
			await _context.SaveChangesAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = "Message sent.";
			return response;
		}

		public async Task<ApiResponse> GetAttendance(string studentId)
		{
			var response = new ApiResponse();

			var attendance = await _context.Attendances
				.Where(a => a.StudentId == studentId)
				.Include(a => a.Course)
				.Select(a => new
				{
					Course = a.Course.Name,
					Date = a.Date,
					IsPresent = a.Status == AttendanceStatus.Present,
				})
				.ToListAsync();

			response.StatusCode = HttpStatusCode.OK;
			response.IsSuccess = true;
			response.Result = attendance;
			return response;
		}
	}
}
