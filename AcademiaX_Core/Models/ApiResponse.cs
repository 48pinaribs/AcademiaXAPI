using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Core.Models
{
	public class ApiResponse
	{
		public ApiResponse()
		{ 
			ErrorMessages = new List<string>();
		}
		public HttpStatusCode StatusCode { get; set; }
		public bool IsSuccess { get; set; }
		public List<string> ErrorMessages { get; set; }
		public object Result { get; set; }
	}
}
