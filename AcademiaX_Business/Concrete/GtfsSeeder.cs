using AcademiaX_Data_Access.Context;
using AcademiaX_Data_Access.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademiaX_Business.Concrete
{
	/// <summary>
	/// Kampüs ulaşım (dolmuş) verisi eskiden diskteki bir GTFS zip dosyasından okunuyordu
	/// (appsettings: GtfsSettings:DataPath). O dosya kaybolunca (veya bulutta hiç var
	/// olmayınca — yerel bir dosya yolu buluta taşınmaz) özellik tamamen çalışmaz oldu.
	/// Artık veri, zaten var olan Stops/Trips/StopTimes tablolarında (veritabanında) tutuluyor
	/// ve uygulama ilk açılışta bu tablolar boşsa gerçekçi örnek bir kampüs hattı ile
	/// dolduruyor — hem bulutta hem yerelde çalışır, dış dosyaya bağımlılık yok.
	/// </summary>
	public static class GtfsSeeder
	{
		public static async Task SeedAsync(ApplicationDbContext context)
		{
			if (await context.Stops.AnyAsync())
			{
				return; // zaten seed edilmiş
			}

			var stops = new List<Stop>
			{
				new() { StopId = "YURT", StopName = "Öğrenci Yurdu", StopLat = 39.9250, StopLon = 32.8450 },
				new() { StopId = "KAMPUS", StopName = "Üniversite Ana Kapısı", StopLat = 39.9320, StopLon = 32.8500 },
				new() { StopId = "KUTUPHANE", StopName = "Merkez Kütüphane", StopLat = 39.9300, StopLon = 32.8480 },
				new() { StopId = "MUHENDISLIK", StopName = "Mühendislik Fakültesi", StopLat = 39.9340, StopLon = 32.8530 },
				new() { StopId = "SEHIR", StopName = "Şehir Merkezi", StopLat = 39.9200, StopLon = 32.8600 },
			};

			// Gidiş: Yurt -> Kampüs -> Kütüphane -> Mühendislik -> Şehir Merkezi (direction 0)
			// Dönüş: aynı hat ters yönde (direction 1)
			var gidisSirasi = new[] { "YURT", "KAMPUS", "KUTUPHANE", "MUHENDISLIK", "SEHIR" };
			var donusSirasi = gidisSirasi.Reverse().ToArray();

			var trips = new List<Trip>();
			var stopTimes = new List<StopTime>();

			void BuildDirection(string[] sira, int directionId)
			{
				// 07:00'dan 21:00'a kadar her saat başı bir dolmuş seferi.
				for (int hour = 7; hour <= 21; hour++)
				{
					var tripId = $"DOLMUS-{directionId}-{hour:00}00";
					trips.Add(new Trip { TripId = tripId, RouteId = "DOLMUS", ServiceId = "HERGUN", DirectionId = directionId });

					var baseTime = TimeSpan.FromHours(hour);
					for (int i = 0; i < sira.Length; i++)
					{
						// Duraklar arası ortalama 6 dakika, durakta 2 dakika bekleme.
						var arrival = baseTime + TimeSpan.FromMinutes(i * 6);
						stopTimes.Add(new StopTime
						{
							TripId = tripId,
							StopId = sira[i],
							StopSequence = i + 1,
							ArrivalTime = arrival,
							DepartureTime = arrival + TimeSpan.FromMinutes(2),
						});
					}
				}
			}

			BuildDirection(gidisSirasi, 0);
			BuildDirection(donusSirasi, 1);

			context.Stops.AddRange(stops);
			context.Trips.AddRange(trips);
			context.StopTimes.AddRange(stopTimes);

			await context.SaveChangesAsync();
		}
	}
}
