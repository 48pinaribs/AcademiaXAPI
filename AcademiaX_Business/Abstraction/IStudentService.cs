using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public  interface IStudentService
	{

		Task<ApiResponse> GetAllStudents();
		Task<ApiResponse> GetStudentById(string studentId);
		Task<ApiResponse> GetProfile(string userId);
		Task<ApiResponse> GetEnrolledCourses(string studentId);
		Task<ApiResponse> GetGrades(string studentId);

		Task<ApiResponse> EnrollCourse(string studentId, int courseId);
		Task<ApiResponse> DropCourse(string studentId, int courseId);

		Task<ApiResponse> SendMessageToAdvisor(string studentId, string message);
		Task<ApiResponse> GetAttendance(string studentId);
	}
}
