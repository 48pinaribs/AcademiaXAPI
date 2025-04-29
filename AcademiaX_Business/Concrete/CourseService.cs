using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Domain;
using AcademiaX_Data_Access.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AcademiaX_Business.Concrete
{
	public class CourseService : ICourseService
	{
		private readonly ApplicationDbContext _context;

		public CourseService(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<ApiResponse> CreateCourse(CreateCourseRequestDTO model)
		{
			var course = new Course
			{
				Name = model.Name,
				Code = model.Code,
				Description = model.Description,
				Credits = model.Credits,
				DepartmentId = model.DepartmentId,
				SemesterId = model.SemesterId,
				TeacherId = model.TeacherId
			};

			_context.Courses.Add(course);
			await _context.SaveChangesAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.Created,
				IsSuccess = true,
				Result = course
			};
		}

		public async Task<ApiResponse> UpdateCourse(UpdateCourseRequestDTO model)
		{
			var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == model.CourseId);

			if (course == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Course not found." }
				};
			}

			course.Name = model.Name;
			course.Description = model.Description;
			course.Credits = model.Credits;
			course.DepartmentId = model.DepartmentId;
			course.SemesterId = model.SemesterId;
			course.TeacherId = model.TeacherId;

			await _context.SaveChangesAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true
			};
		}

		public async Task<ApiResponse> DeleteCourse(int courseId)
		{
			var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);

			if (course == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Course not found." }
				};
			}

			_context.Courses.Remove(course);
			await _context.SaveChangesAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.NoContent,
				IsSuccess = true
			};
		}

		public async Task<ApiResponse> GetAllCourses()
		{
			var courses = await _context.Courses
				.Select(c => new CourseDTO
				{
					CourseId = c.Id,
					Name = c.Name,
					Code = c.Code,
					Description = c.Description,
					TotalStudents = c.Students.Count()
				}).ToListAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = courses
			};
		}

		public async Task<ApiResponse> GetCourseById(int courseId)
		{
			var course = await _context.Courses
				.Where(c => c.Id == courseId)
				.Select(c => new CourseDTO
				{
					CourseId = c.Id,
					Name = c.Name,
					Code = c.Code,
					Description = c.Description,
					TotalStudents = c.Students.Count()
				})
				.FirstOrDefaultAsync();

			if (course == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Course not found." }
				};
			}

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = course
			};
		}

		public async Task<ApiResponse> EnrollInCourse(EnrollInCourseRequestDTO model)
		{
			// 1. Kursu bul
			var course = await _context.Courses
				.Include(c => c.Students)
				.FirstOrDefaultAsync(c => c.Id == model.CourseId);

			if (course == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Course not found." }
				};
			}

			// 2. Öğrenci zaten kayıtlı mı kontrol et
			bool alreadyEnrolled = course.Students.Any(s => s.Id == model.StudentId);

			if (alreadyEnrolled)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.BadRequest,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Student already enrolled in this course." }
				};
			}

			// 3. Öğrenciyi getir
			var student = await _context.Users.FirstOrDefaultAsync(u => u.Id == model.StudentId);

			if (student == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Student not found." }
				};
			}

			// 4. Öğrenciyi kursun Students koleksiyonuna ekle
			course.Students.Add(student);

			// 5. Değişiklikleri kaydet
			await _context.SaveChangesAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = "Student successfully enrolled."
			};
		}


		public async Task<ApiResponse> UnenrollFromCourse(UnenrollFromCourseRequestDTO model)
		{
			// 1. Kursu Students ile birlikte getir
			var course = await _context.Courses
				.Include(c => c.Students)
				.FirstOrDefaultAsync(c => c.Id == model.CourseId);

			if (course == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Course not found." }
				};
			}

			// 2. Öğrenciyi bul
			var student = course.Students.FirstOrDefault(s => s.Id == model.StudentId);

			if (student == null)
			{
				return new ApiResponse
				{
					StatusCode = HttpStatusCode.NotFound,
					IsSuccess = false,
					ErrorMessages = new List<string> { "Student not enrolled in this course." }
				};
			}

			// 3. Öğrenciyi kursun Students listesinden çıkar
			course.Students.Remove(student);

			// 4. Değişiklikleri kaydet
			await _context.SaveChangesAsync();

			// 5. Başarı mesajı
			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = "Student successfully unenrolled from the course."
			};
		}


		public async Task<ApiResponse> GetEnrolledCourses(string userId)
		{
			var enrolledCourses = await _context.Courses
				.Where(c => c.Students.Any(s => s.Id == userId))
				.Select(c => new CourseDTO
				{
					CourseId = c.Id,
					Name = c.Name,
					Code = c.Code,
					Description = c.Description,
					TotalStudents = c.Students.Count()
				})
				.ToListAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = enrolledCourses
			};
		}


		public async Task<ApiResponse> GetAvailableCourses(string userId)
		{
			// 1. Önce öğrencinin kayıtlı olduğu kursların ID'lerini bul
			var enrolledCourseIds = await _context.Courses
				.Where(c => c.Students.Any(s => s.Id == userId))
				.Select(c => c.Id)
				.ToListAsync();

			// 2. Sonra kayıtlı olmadığı kursları getir
			var availableCourses = await _context.Courses
				.Where(c => !enrolledCourseIds.Contains(c.Id))
				.Select(c => new CourseDTO
				{
					CourseId = c.Id,
					Name = c.Name,
					Code = c.Code,
					Description = c.Description,
					TotalStudents = c.Students.Count()
				})
				.ToListAsync();

			return new ApiResponse
			{
				StatusCode = HttpStatusCode.OK,
				IsSuccess = true,
				Result = availableCourses
			};
		}

	}
}
