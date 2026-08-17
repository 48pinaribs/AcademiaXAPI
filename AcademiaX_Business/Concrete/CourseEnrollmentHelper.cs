using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Enums;
using AcademiaX_Data_Access.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaX_Business.Concrete
{
	/// <summary>
	/// "Öğrenciyi derse ekle" mantığı önceden CourseService.EnrollInCourse, StudentService.EnrollCourse
	/// ve TeacherService.AssignStudentToCourse içinde üç kez, birbirinden hafifçe farklı şekilde
	/// tekrarlanmıştı. Tek doğruluk kaynağı burası; her servis kendi ApiResponse şekline çevirir.
	/// </summary>
	internal static class CourseEnrollmentHelper
	{
		internal readonly record struct Result(bool Success, string ErrorMessage, bool NotFound);

		internal static async Task<Result> EnrollStudentAsync(
			ApplicationDbContext context,
			int courseId,
			string studentId,
			bool requireStudentRole)
		{
			bool courseExists = await context.Courses.AnyAsync(c => c.Id == courseId);
			if (!courseExists)
			{
				return new Result(false, "Course not found.", NotFound: true);
			}

			var student = await context.ApplicationUsers.FindAsync(studentId);
			if (student == null || (requireStudentRole && student.UserType != UserType.Student))
			{
				return new Result(false, requireStudentRole ? "Invalid student." : "Student not found.", NotFound: true);
			}

			bool alreadyEnrolled = await context.Courses
				.Where(c => c.Id == courseId)
				.SelectMany(c => c.Students)
				.AnyAsync(s => s.Id == studentId);
			if (alreadyEnrolled)
			{
				return new Result(false, "Student already enrolled in this course.", NotFound: false);
			}

			var course = await context.Courses.Include(c => c.Students).FirstAsync(c => c.Id == courseId);
			course.Students.Add(student);
			await context.SaveChangesAsync();

			return new Result(true, null, NotFound: false);
		}
	}
}
