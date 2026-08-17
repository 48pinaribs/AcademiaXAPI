using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AcademiaX_Business.Dtos.Gtfs
{
	/// <summary>
	/// Admin'in ring zamanlamasını sıfırdan yeniden oluşturmak için gönderdiği parametreler
	/// — bkz. GtfsService.RegenerateSchedule. Var olan tüm Trip/StopTime kayıtlarının yerine
	/// geçer; Stops tablosuna dokunmaz.
	/// </summary>
	public class RegenerateScheduleRequestDTO
	{
		// Gidiş yönündeki durak sırası (en az 2 durak) — dönüş bunun tersine çevrilmiş hali.
		[Required]
		[MinLength(2, ErrorMessage = "Güzergah en az 2 durak içermelidir.")]
		public List<string> StopIdsInOrder { get; set; }

		[Range(0, 23)]
		public int StartHour { get; set; } = 7;

		[Range(0, 23)]
		public int EndHour { get; set; } = 21;

		[Range(1, 1440)]
		public int IntervalMinutes { get; set; } = 60;

		[Range(1, 120)]
		public int MinutesBetweenStops { get; set; } = 6;

		[Range(0, 60)]
		public int DwellMinutes { get; set; } = 2;
	}
}
