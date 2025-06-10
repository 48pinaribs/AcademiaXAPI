using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Dtos.Gtfs
{
	public class TripDTO
	{
		public string TripId { get; set; }
		public string RouteId { get; set; }
		public string ServiceId { get; set; }
		public int DirectionId { get; set; }  // 0 = gidiş, 1 = dönüş
	}
}
