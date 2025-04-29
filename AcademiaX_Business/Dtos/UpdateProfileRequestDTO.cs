using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos
{
	public class UpdateProfileRequestDTO
	{
		public string Id { get; set; }
		public string  Image { get; set; }
		public string Address { get; set; }
		public string FirstName { get; set; }
		public string FullName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
	}
}
