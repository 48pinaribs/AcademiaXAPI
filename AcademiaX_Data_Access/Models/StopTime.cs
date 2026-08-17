using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Data_Access.Models
{
	public class StopTime
	{
		// Not: bu tablo önceden HasNoKey() ile "keyless" tanımlıydı (sadece okunacak bir görünüm
		// gibi) — bu yüzden EF Core seed verisini normal yoldan ekleyemiyordu. Gerçek bir Id
		// (identity) eklendi ki tablo normal bir şekilde yazılabilsin.
		public int Id { get; set; }
		public string TripId { get; set; }
		public TimeSpan ArrivalTime { get; set; }
		public TimeSpan DepartureTime { get; set; }
		public string StopId { get; set; }
		public int StopSequence { get; set; }
	}
}
