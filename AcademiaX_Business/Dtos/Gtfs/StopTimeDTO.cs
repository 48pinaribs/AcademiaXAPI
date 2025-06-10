using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Gtfs
{
	public class StopTimeDTO
	{
		public string TripId { get; set; }
		public string ArrivalTime { get; set; }     // HH:mm:ss formatında string
		public string DepartureTime { get; set; }   // HH:mm:ss formatında string
		public string StopId { get; set; }
		public int StopSequence { get; set; }
	}
}
