using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Gtfs;
using AcademiaX_Core.Models;
using AcademiaX_Data_Access.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AcademiaX_Business.Concrete
{
	public class GtfsService : IGtfsService
	{
		private const string StopsCacheKey = "gtfs:stops";
		private const string TripsCacheKey = "gtfs:trips";
		private const string StopTimesCacheKey = "gtfs:stop_times";
		private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

		private readonly ApplicationDbContext _context;
		private readonly IMemoryCache _cache;
		private readonly ILogger<GtfsService> _logger;

		// Not: eskiden bu sınıf diskteki bir GTFS zip dosyasını (appsettings: GtfsSettings:DataPath)
		// okuyordu. Dosya yerel bir yoldaydı, bulut ortamında hiç var olmuyordu ve yerelde de
		// kaybolabiliyordu — nitekim öyle oldu. Artık veri, uygulamayla birlikte taşınan
		// Stops/Trips/StopTimes tablolarından (bkz. GtfsSeeder) okunuyor.
		public GtfsService(ApplicationDbContext context, IMemoryCache cache, ILogger<GtfsService> logger)
		{
			_context = context;
			_cache = cache;
			_logger = logger;
		}

		private async Task<List<StopDTO>> LoadStopsAsync()
		{
			if (_cache.TryGetValue(StopsCacheKey, out List<StopDTO> cached))
			{
				return cached;
			}

			var stops = await _context.Stops
				.Select(s => new StopDTO
				{
					StopId = s.StopId,
					StopName = s.StopName,
					StopLat = s.StopLat,
					StopLon = s.StopLon,
				})
				.ToListAsync();

			_cache.Set(StopsCacheKey, stops, CacheDuration);
			return stops;
		}

		private async Task<List<TripDTO>> LoadTripsAsync()
		{
			if (_cache.TryGetValue(TripsCacheKey, out List<TripDTO> cached))
			{
				return cached;
			}

			var trips = await _context.Trips
				.Select(t => new TripDTO
				{
					TripId = t.TripId,
					RouteId = t.RouteId,
					ServiceId = t.ServiceId,
					DirectionId = t.DirectionId,
				})
				.ToListAsync();

			_cache.Set(TripsCacheKey, trips, CacheDuration);
			return trips;
		}

		private async Task<List<StopTimeDTO>> LoadStopTimesAsync()
		{
			if (_cache.TryGetValue(StopTimesCacheKey, out List<StopTimeDTO> cached))
			{
				return cached;
			}

			var stopTimes = await _context.StopTimes
				.Select(st => new StopTimeDTO
				{
					TripId = st.TripId,
					StopId = st.StopId,
					StopSequence = st.StopSequence,
					ArrivalTime = st.ArrivalTime.ToString(@"hh\:mm\:ss"),
					DepartureTime = st.DepartureTime.ToString(@"hh\:mm\:ss"),
				})
				.ToListAsync();

			_cache.Set(StopTimesCacheKey, stopTimes, CacheDuration);
			return stopTimes;
		}

		public async Task<ApiResponse> GetStops()
		{
			var response = new ApiResponse();
			try
			{
				response.Result = await LoadStopsAsync();
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(GetStops));
			}

			return response;
		}

		public async Task<ApiResponse> GetTrips()
		{
			var response = new ApiResponse();
			try
			{
				response.Result = await LoadTripsAsync();
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(GetTrips));
			}

			return response;
		}

		public async Task<ApiResponse> GetStopTimes()
		{
			var response = new ApiResponse();
			try
			{
				response.Result = await LoadStopTimesAsync();
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(GetStopTimes));
			}

			return response;
		}

		public async Task<ApiResponse> GetStopTimeTable(string stopId, int directionId = 0)
		{
			var response = new ApiResponse();
			try
			{
				var trips = (await LoadTripsAsync())
					.Where(t => t.DirectionId == directionId)
					.Select(t => t.TripId)
					.ToHashSet();

				var stopTimes = (await LoadStopTimesAsync())
					.Where(st => st.StopId == stopId && trips.Contains(st.TripId))
					.OrderBy(st => st.ArrivalTime)
					.ToList();

				response.Result = stopTimes;
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(GetStopTimeTable));
			}

			return response;
		}

		public async Task<ApiResponse> GetRoutePlan(string fromStopId, string toStopId, string afterTime = null)
		{
			var response = new ApiResponse();
			try
			{
				if (string.IsNullOrWhiteSpace(fromStopId) || string.IsNullOrWhiteSpace(toStopId))
				{
					response.StatusCode = System.Net.HttpStatusCode.BadRequest;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Kalkış ve varış durağı seçilmelidir.");
					return response;
				}

				if (fromStopId == toStopId)
				{
					response.StatusCode = System.Net.HttpStatusCode.BadRequest;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Kalkış ve varış durağı aynı olamaz.");
					return response;
				}

				var stops = await LoadStopsAsync();
				var fromStop = stops.FirstOrDefault(s => s.StopId == fromStopId);
				var toStop = stops.FirstOrDefault(s => s.StopId == toStopId);
				if (fromStop == null || toStop == null)
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Durak bulunamadı.");
					return response;
				}

				var trips = await LoadTripsAsync();
				var stopTimes = await LoadStopTimesAsync();

				// Her yön (direction) için tek bir örnek sefer üzerinden durak sırasını çıkar —
				// hat sabit olduğu için (bkz. GtfsSeeder) bir yöndeki tüm seferler aynı durak
				// sırasını izler, hangi yönün from->to'yu kapsadığını bulmak için bu yeterli.
				int? validDirection = null;
				foreach (var directionId in trips.Select(t => t.DirectionId).Distinct())
				{
					var sampleTripId = trips.First(t => t.DirectionId == directionId).TripId;
					var sampleStops = stopTimes.Where(st => st.TripId == sampleTripId).ToList();
					var fromSeq = sampleStops.FirstOrDefault(st => st.StopId == fromStopId)?.StopSequence;
					var toSeq = sampleStops.FirstOrDefault(st => st.StopId == toStopId)?.StopSequence;
					if (fromSeq.HasValue && toSeq.HasValue && fromSeq < toSeq)
					{
						validDirection = directionId;
						break;
					}
				}

				if (validDirection == null)
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Bu duraklar arasında bir güzergah bulunamadı.");
					return response;
				}

				var directionTripIds = trips.Where(t => t.DirectionId == validDirection).Select(t => t.TripId).ToHashSet();

				// Her sefer (trip) için kalkış (fromStop) ve varış (toStop) saatlerini eşleştir.
				var candidates = stopTimes
					.Where(st => st.StopId == fromStopId && directionTripIds.Contains(st.TripId))
					.Select(fromSt => new
					{
						FromSt = fromSt,
						ToSt = stopTimes.FirstOrDefault(st => st.TripId == fromSt.TripId && st.StopId == toStopId)
					})
					.Where(x => x.ToSt != null)
					.OrderBy(x => x.FromSt.DepartureTime, StringComparer.Ordinal)
					.ToList();

				if (candidates.Count == 0)
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Bu duraklar arasında planlanmış bir sefer bulunamadı.");
					return response;
				}

				var effectiveAfter = string.IsNullOrWhiteSpace(afterTime) ? DateTime.Now.ToString("HH:mm:ss") : afterTime;
				var next = candidates.FirstOrDefault(x => string.CompareOrdinal(x.FromSt.DepartureTime, effectiveAfter) >= 0);
				var isNextDay = next == null;
				if (next == null)
				{
					next = candidates.First();
				}

				var departure = TimeSpan.Parse(next.FromSt.DepartureTime);
				var arrival = TimeSpan.Parse(next.ToSt.ArrivalTime);
				var now = TimeSpan.Parse(effectiveAfter);

				response.Result = new Dtos.Gtfs.RoutePlanDTO
				{
					FromStopId = fromStop.StopId,
					FromStopName = fromStop.StopName,
					ToStopId = toStop.StopId,
					ToStopName = toStop.StopName,
					DirectionId = validDirection.Value,
					DepartureTime = next.FromSt.DepartureTime,
					ArrivalTime = next.ToSt.ArrivalTime,
					DurationMinutes = (int)(arrival - departure).TotalMinutes,
					WaitMinutes = isNextDay ? 0 : (int)Math.Round((departure - now).TotalMinutes),
					IsNextDay = isNextDay,
				};
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(GetRoutePlan));
			}

			return response;
		}

		private void InvalidateCache()
		{
			_cache.Remove(StopsCacheKey);
			_cache.Remove(TripsCacheKey);
			_cache.Remove(StopTimesCacheKey);
		}

		public async Task<ApiResponse> UpsertStop(Dtos.Gtfs.UpsertStopRequestDTO model)
		{
			var response = new ApiResponse();
			try
			{
				var stop = await _context.Stops.FirstOrDefaultAsync(s => s.StopId == model.StopId);
				var isNew = stop == null;
				if (isNew)
				{
					stop = new AcademiaX_Data_Access.Models.Stop { StopId = model.StopId };
					_context.Stops.Add(stop);
				}

				stop.StopName = model.StopName;
				stop.StopLat = model.StopLat;
				stop.StopLon = model.StopLon;

				await _context.SaveChangesAsync();
				InvalidateCache();

				response.StatusCode = System.Net.HttpStatusCode.OK;
				response.IsSuccess = true;
				response.Result = isNew ? "Durak eklendi." : "Durak güncellendi.";
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(UpsertStop));
			}

			return response;
		}

		public async Task<ApiResponse> DeleteStop(string stopId)
		{
			var response = new ApiResponse();
			try
			{
				var stop = await _context.Stops.FirstOrDefaultAsync(s => s.StopId == stopId);
				if (stop == null)
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Durak bulunamadı.");
					return response;
				}

				// Bu durağa ait StopTime kayıtları da temizlenir; kalan Trip'lerin sırası
				// bozulur (eksik bir durakla kalır) — admin genelde bunun ardından
				// "Zamanlamayı Yeniden Oluştur"u çalıştırmalı.
				var relatedStopTimes = _context.StopTimes.Where(st => st.StopId == stopId);
				_context.StopTimes.RemoveRange(relatedStopTimes);
				_context.Stops.Remove(stop);

				await _context.SaveChangesAsync();
				InvalidateCache();

				response.StatusCode = System.Net.HttpStatusCode.OK;
				response.IsSuccess = true;
				response.Result = "Durak silindi.";
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(DeleteStop));
			}

			return response;
		}

		public async Task<ApiResponse> RegenerateSchedule(Dtos.Gtfs.RegenerateScheduleRequestDTO model)
		{
			var response = new ApiResponse();
			try
			{
				if (model.EndHour < model.StartHour)
				{
					response.StatusCode = System.Net.HttpStatusCode.BadRequest;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Bitiş saati başlangıç saatinden önce olamaz.");
					return response;
				}

				var existingStopIds = (await _context.Stops.Select(s => s.StopId).ToListAsync()).ToHashSet();
				var missing = model.StopIdsInOrder.Where(id => !existingStopIds.Contains(id)).ToList();
				if (missing.Count > 0)
				{
					response.StatusCode = System.Net.HttpStatusCode.BadRequest;
					response.IsSuccess = false;
					response.ErrorMessages.Add($"Tanımlı olmayan durak(lar): {string.Join(", ", missing)}");
					return response;
				}

				// Var olan tüm sefer verisini sil, yerine yenisini koy.
				_context.StopTimes.RemoveRange(_context.StopTimes);
				_context.Trips.RemoveRange(_context.Trips);
				await _context.SaveChangesAsync();

				var (trips, stopTimes) = GtfsSeeder.BuildSchedule(
					model.StopIdsInOrder.ToArray(),
					model.StartHour,
					model.EndHour,
					model.IntervalMinutes,
					model.MinutesBetweenStops,
					model.DwellMinutes);

				_context.Trips.AddRange(trips);
				_context.StopTimes.AddRange(stopTimes);
				await _context.SaveChangesAsync();
				InvalidateCache();

				response.StatusCode = System.Net.HttpStatusCode.OK;
				response.IsSuccess = true;
				response.Result = $"{trips.Count} sefer, {stopTimes.Count} durak-zamanı oluşturuldu.";
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(RegenerateSchedule));
			}

			return response;
		}

		public async Task<ApiResponse> DeleteTrip(string tripId)
		{
			var response = new ApiResponse();
			try
			{
				var trip = await _context.Trips.FirstOrDefaultAsync(t => t.TripId == tripId);
				if (trip == null)
				{
					response.StatusCode = System.Net.HttpStatusCode.NotFound;
					response.IsSuccess = false;
					response.ErrorMessages.Add("Sefer bulunamadı.");
					return response;
				}

				var relatedStopTimes = _context.StopTimes.Where(st => st.TripId == tripId);
				_context.StopTimes.RemoveRange(relatedStopTimes);
				_context.Trips.Remove(trip);

				await _context.SaveChangesAsync();
				InvalidateCache();

				response.StatusCode = System.Net.HttpStatusCode.OK;
				response.IsSuccess = true;
				response.Result = "Sefer iptal edildi.";
			}
			catch (Exception ex)
			{
				LogAndSetGenericError(response, ex, nameof(DeleteTrip));
			}

			return response;
		}

		private void LogAndSetGenericError(ApiResponse response, Exception ex, string operation)
		{
			_logger.LogError(ex, "GtfsService.{Operation} failed", operation);
			response.IsSuccess = false;
			response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
			response.ErrorMessages.Add("Ulaşım verisi alınırken bir hata oluştu.");
		}
	}
}
