using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class Stop
	{
		public string StopId { get; set; }
		public string StopName { get; set; }
		public double StopLat { get; set; }
		public double StopLon { get; set; }
	}
}
