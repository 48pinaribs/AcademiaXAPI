using AcademiaX_Business.Dtos.Courses;
using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface ICourseService
	{
		Task<ApiResponse> CreateCourse(CreateCourseRequestDTO model);
		Task<ApiResponse> UpdateCourse(UpdateCourseRequestDTO model);
		Task<ApiResponse> DeleteCourse(int courseId);
		Task<ApiResponse> GetAllCourses();
		Task<ApiResponse> GetCourseById(int courseId);
		Task<ApiResponse> EnrollInCourse(EnrollInCourseRequestDTO model);
		Task<ApiResponse> UnenrollFromCourse(UnenrollFromCourseRequestDTO model);
		Task<ApiResponse> GetEnrolledCourses(string userId);
		Task<ApiResponse> GetAvailableCourses(string userId);
		Task<ApiResponse> GetStudentsInCourse(int courseId);

	}
}
