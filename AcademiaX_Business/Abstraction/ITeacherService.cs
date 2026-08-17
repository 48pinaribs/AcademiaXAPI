using AcademiaX_Business.Dtos;
using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface ITeacherService
	{

		Task<ApiResponse> GetAllTeachers();
		Task<ApiResponse> GetTeacherById(string teacherId);

		/// Öğretmenin verdiği dersleri getirir
		Task<ApiResponse> GetCoursesByTeacher(TeacherCoursesDTO model);

		/// Öğretmenin profil bilgilerini getirir
		Task<ApiResponse> GetTeacherProfile(TeacherProfileDTO model);

		/// Öğretmen bilgilerini günceller
		Task<ApiResponse> UpdateTeacherProfile(UpdateProfileRequestDTO model);

		/// Öğrenciyi derse atar
		Task<ApiResponse> AssignStudentToCourse(EnrollInCourseRequestDTO model);

		/// Öğretmenin gelen kutusundaki mesajları (öğrencilerden) getirir
		Task<ApiResponse> GetMessages(string teacherId);

		// --- Not girişi (yalnızca dersin öğretmeni ya da Admin) ---
		Task<ApiResponse> GetGradesForCourse(int courseId, string requestingUserId, bool isAdmin);
		Task<ApiResponse> UpsertGrade(UpsertGradeRequestDTO model, string requestingUserId, bool isAdmin);

		// --- Yoklama alma (yalnızca dersin öğretmeni ya da Admin) ---
		Task<ApiResponse> GetAttendanceForCourseDate(int courseId, System.DateTime date, string requestingUserId, bool isAdmin);
		Task<ApiResponse> MarkAttendance(BulkMarkAttendanceRequestDTO model, string requestingUserId, bool isAdmin);
	}
}
