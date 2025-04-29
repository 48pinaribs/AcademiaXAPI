using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Courses
{
	public class CreateCourseRequestDTO
	{
		public string Name { get; set; }

		public string Code { get; set; }

		public string Description { get; set; }
		
		public int Credits { get; set; }
		
		public int DepartmentId { get; set; }
		
		public int SemesterId { get; set; }
		
		public string TeacherId { get; set; }
	}
}
