using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos
{
	public class RegisterRequestDTO
	{
			public string UserName { get; set; }
			public string Fullname { get; set; }
			public string Password { get; set; }
			public string ConfirmPassword { get; set; }
			public string UserType { get; set; } // Student, Teacher, Admin
			public string Email { get; set; }
			public string PhoneNumber { get; set; }
			public DateTime? BirthDate { get; set; }
			public string SecurityQuestion { get; set; }
			public string SecurityAnswer { get; set; }
		
	}
}
