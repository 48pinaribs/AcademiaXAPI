namespace AcademiaX_Business.Dtos.Gtfs
{
	/// <summary>İki durak arasındaki bir sonraki seferin planı (bkz. GtfsService.GetRoutePlan).</summary>
	public class RoutePlanDTO
	{
		public string FromStopId { get; set; }
		public string FromStopName { get; set; }
		public string ToStopId { get; set; }
		public string ToStopName { get; set; }
		public int DirectionId { get; set; }
		public string DepartureTime { get; set; } // HH:mm:ss
		public string ArrivalTime { get; set; }   // HH:mm:ss
		public int DurationMinutes { get; set; }
		public int WaitMinutes { get; set; }
		// Bugünün seferleri bittiyse, ilk sefer gösterilir ve bu true olur.
		public bool IsNextDay { get; set; }
	}
}
