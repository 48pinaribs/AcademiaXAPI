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
		/// Öğretmenin verdiği dersleri getirir
		Task<ApiResponse> GetCoursesByTeacher(TeacherCoursesDTO model);

		/// Öğretmenin profil bilgilerini getirir
		Task<ApiResponse> GetTeacherProfile(TeacherProfileDTO model);

		/// Öğretmen bilgilerini günceller
		Task<ApiResponse> UpdateTeacherProfile(UpdateProfileRequestDTO model);

		/// Öğrenciyi derse atar
		Task<ApiResponse> AssignStudentToCourse(EnrollInCourseRequestDTO model);
	}
}
