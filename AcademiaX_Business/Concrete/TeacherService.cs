using AcademiaX_Core.Models;
using AcademiaX_Business.Abstraction;
using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Domain;
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

	// Course.TeacherId == requestingUserId değilse ve admin de değilse, bu dersle ilgili
	// yazma/okuma işlemi reddedilir. Hem not girişi hem yoklama alma için ortak kontrol.
	private async Task<(bool Allowed, ApiResponse Denied)> CanManageCourseAsync(int courseId, string requestingUserId, bool isAdmin)
	{
		var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId);
		if (course == null)
		{
			return (false, new ApiResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false, ErrorMessages = { "Ders bulunamadı." } });
		}

		if (!isAdmin && course.TeacherId != requestingUserId)
		{
			return (false, new ApiResponse { StatusCode = HttpStatusCode.Forbidden, IsSuccess = false, ErrorMessages = { "Bu dersin öğretmeni değilsiniz." } });
		}

		return (true, null);
	}

	public async Task<ApiResponse> GetGradesForCourse(int courseId, string requestingUserId, bool isAdmin)
	{
		var response = new ApiResponse();

		var (allowed, denied) = await CanManageCourseAsync(courseId, requestingUserId, isAdmin);
		if (!allowed) return denied;

		var grades = await _context.Grades
			.Where(g => g.CourseId == courseId)
			.Include(g => g.Student)
			.Select(g => new GradeDTO
			{
				Id = g.Id,
				StudentId = g.StudentId,
				StudentName = g.Student.FirstName + " " + g.Student.LastName,
				CourseId = g.CourseId,
				ExamType = g.ExamType.ToString(),
				Value = g.Value
			})
			.ToListAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = grades;
		return response;
	}

	public async Task<ApiResponse> UpsertGrade(UpsertGradeRequestDTO model, string requestingUserId, bool isAdmin)
	{
		var response = new ApiResponse();

		var (allowed, denied) = await CanManageCourseAsync(model.CourseId, requestingUserId, isAdmin);
		if (!allowed) return denied;

		if (!Enum.TryParse<ExamType>(model.ExamType, out var examType))
		{
			response.StatusCode = HttpStatusCode.BadRequest;
			response.IsSuccess = false;
			response.ErrorMessages.Add("Geçersiz sınav türü. (Midterm, Final, Resit)");
			return response;
		}

		var student = await _context.ApplicationUsers
			.FirstOrDefaultAsync(u => u.Id == model.StudentId && u.UserType == UserType.Student);
		if (student == null)
		{
			response.StatusCode = HttpStatusCode.NotFound;
			response.IsSuccess = false;
			response.ErrorMessages.Add("Öğrenci bulunamadı.");
			return response;
		}

		var isEnrolled = await _context.Courses
			.Where(c => c.Id == model.CourseId)
			.SelectMany(c => c.Students)
			.AnyAsync(s => s.Id == model.StudentId);
		if (!isEnrolled)
		{
			response.StatusCode = HttpStatusCode.BadRequest;
			response.IsSuccess = false;
			response.ErrorMessages.Add("Öğrenci bu derse kayıtlı değil.");
			return response;
		}

		var grade = await _context.Grades.FirstOrDefaultAsync(g =>
			g.StudentId == model.StudentId && g.CourseId == model.CourseId && g.ExamType == examType);

		if (grade == null)
		{
			grade = new Grade
			{
				StudentId = model.StudentId,
				CourseId = model.CourseId,
				ExamType = examType,
			};
			_context.Grades.Add(grade);
		}

		grade.Value = model.Value;
		// TotalGrade şimdilik tek bir sınav türünü yansıtıyor — Vize/Final ağırlıklandırması
		// (örn. %40 Vize + %60 Final) ileride bir iş kuralı olarak eklenebilir.
		grade.TotalGrade = model.Value;

		await _context.SaveChangesAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = "Not kaydedildi.";
		return response;
	}

	public async Task<ApiResponse> GetAttendanceForCourseDate(int courseId, DateTime date, string requestingUserId, bool isAdmin)
	{
		var response = new ApiResponse();

		var (allowed, denied) = await CanManageCourseAsync(courseId, requestingUserId, isAdmin);
		if (!allowed) return denied;

		// Npgsql "timestamp with time zone" için Kind=Utc şart — query string'ten gelen DateTime
		// Kind=Unspecified oluyor, aksi halde InvalidCastException fırlıyordu.
		var dateOnly = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc);
		var records = await _context.Attendances
			.Where(a => a.CourseId == courseId && a.Date == dateOnly)
			.Select(a => new AttendanceRecordDTO { StudentId = a.StudentId, Status = a.Status.ToString() })
			.ToListAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = records;
		return response;
	}

	public async Task<ApiResponse> MarkAttendance(BulkMarkAttendanceRequestDTO model, string requestingUserId, bool isAdmin)
	{
		var response = new ApiResponse();

		var (allowed, denied) = await CanManageCourseAsync(model.CourseId, requestingUserId, isAdmin);
		if (!allowed) return denied;

		var enrolledIds = await _context.Courses
			.Where(c => c.Id == model.CourseId)
			.SelectMany(c => c.Students)
			.Select(s => s.Id)
			.ToListAsync();
		var enrolledSet = enrolledIds.ToHashSet();

		var invalidStudentIds = model.Records.Select(r => r.StudentId).Where(id => !enrolledSet.Contains(id)).ToList();
		if (invalidStudentIds.Count > 0)
		{
			response.StatusCode = HttpStatusCode.BadRequest;
			response.IsSuccess = false;
			response.ErrorMessages.Add("Bazı öğrenciler bu derse kayıtlı değil.");
			return response;
		}

		var dateOnly = DateTime.SpecifyKind(model.Date.Date, DateTimeKind.Utc);
		var existing = await _context.Attendances
			.Where(a => a.CourseId == model.CourseId && a.Date == dateOnly)
			.ToListAsync();
		var existingByStudent = existing.ToDictionary(a => a.StudentId);

		foreach (var record in model.Records)
		{
			if (!Enum.TryParse<AttendanceStatus>(record.Status, out var status))
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				response.IsSuccess = false;
				response.ErrorMessages.Add($"Geçersiz durum: {record.Status} (Present, Absent, Excused)");
				return response;
			}

			if (existingByStudent.TryGetValue(record.StudentId, out var attendance))
			{
				attendance.Status = status;
			}
			else
			{
				_context.Attendances.Add(new Attendance
				{
					StudentId = record.StudentId,
					CourseId = model.CourseId,
					Date = dateOnly,
					Status = status,
				});
			}
		}

		await _context.SaveChangesAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = $"{model.Records.Count} öğrenci için yoklama kaydedildi.";
		return response;
	}

	public async Task<ApiResponse> GetMessages(string teacherId)
	{
		var response = new ApiResponse();

		var messages = await _context.Messages
			.Where(m => m.ReceiverId == teacherId)
			.Include(m => m.Sender)
			.OrderByDescending(m => m.SentAt)
			.Select(m => new MessageDTO
			{
				Id = m.Id,
				SenderId = m.SenderId,
				SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
				Content = m.Content,
				SentAt = m.SentAt,
				IsRead = m.IsRead
			})
			.ToListAsync();

		response.StatusCode = HttpStatusCode.OK;
		response.IsSuccess = true;
		response.Result = messages;
		return response;
	}
}
