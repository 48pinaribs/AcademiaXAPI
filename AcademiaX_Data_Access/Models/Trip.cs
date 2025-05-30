using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class Trip
	{
		public string TripId { get; set; }
		public string RouteId { get; set; }
		public string ServiceId { get; set; }
		public int DirectionId { get; set; }
	}
}
