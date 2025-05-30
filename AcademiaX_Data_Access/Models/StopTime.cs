using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class StopTime
	{
		public string TripId { get; set; }
		public TimeSpan ArrivalTime { get; set; }
		public TimeSpan DepartureTime { get; set; }
		public string StopId { get; set; }
		public int StopSequence { get; set; }
	}
}
