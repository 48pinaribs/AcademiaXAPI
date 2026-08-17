using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos.Gtfs
{
	public class UpsertStopRequestDTO
	{
		[Required]
		[StringLength(50)]
		public string StopId { get; set; }

		[Required]
		[StringLength(150)]
		public string StopName { get; set; }

		[Range(-90, 90)]
		public double StopLat { get; set; }

		[Range(-180, 180)]
		public double StopLon { get; set; }
	}
}
