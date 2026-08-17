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

		private void LogAndSetGenericError(ApiResponse response, Exception ex, string operation)
		{
			_logger.LogError(ex, "GtfsService.{Operation} failed", operation);
			response.IsSuccess = false;
			response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
			response.ErrorMessages.Add("Ulaşım verisi alınırken bir hata oluştu.");
		}
	}
}
