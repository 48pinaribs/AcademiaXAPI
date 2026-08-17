using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Gtfs;
using AcademiaX_Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX.Controllers
{
	// Kampüs ulaşım verisi kişisel veri içermiyor; yine de anonim taramayı/otomasyonu
	// engellemek için en azından giriş yapmış olma şartı aranıyor.
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class GtfsController : ControllerBase
	{
		private readonly IGtfsService _gtfsService;

		public GtfsController(IGtfsService gtfsService)
		{
			_gtfsService = gtfsService;
		}

		[HttpGet("stops")]
		public async Task<ApiResponse> GetStops()
		{
			return await _gtfsService.GetStops();
		}

		[HttpGet("trips")]
		public async Task<ApiResponse> GetTrips()
		{
			return await _gtfsService.GetTrips();
		}

		[HttpGet("stop-times")]
		public async Task<ApiResponse> GetStopTimes()
		{
			return await _gtfsService.GetStopTimes();
		}

		[HttpGet("timetable")]
		public async Task<ApiResponse> GetStopTimeTable([FromQuery] string stopId, [FromQuery] int directionId = 0)
		{
			return await _gtfsService.GetStopTimeTable(stopId, directionId);
		}

		// GET /api/gtfs/plan?fromStopId=YURT&toStopId=SEHIR — "A'dan B'ye nasıl giderim" rota planlayıcısı.
		[HttpGet("plan")]
		public async Task<ApiResponse> GetRoutePlan([FromQuery] string fromStopId, [FromQuery] string toStopId, [FromQuery] string afterTime = null)
		{
			return await _gtfsService.GetRoutePlan(fromStopId, toStopId, afterTime);
		}

		// --- Admin: ring yönetimi ---

		[HttpPost("stops")]
		[Authorize(Roles = "Administrator")]
		public async Task<ApiResponse> UpsertStop([FromBody] UpsertStopRequestDTO model)
		{
			return await _gtfsService.UpsertStop(model);
		}

		[HttpDelete("stops/{stopId}")]
		[Authorize(Roles = "Administrator")]
		public async Task<ApiResponse> DeleteStop(string stopId)
		{
			return await _gtfsService.DeleteStop(stopId);
		}

		// Var olan tüm sefer/zamanlama verisini silip yenisini üretir (bkz. GtfsSeeder.BuildSchedule).
		[HttpPost("schedule/regenerate")]
		[Authorize(Roles = "Administrator")]
		public async Task<ApiResponse> RegenerateSchedule([FromBody] RegenerateScheduleRequestDTO model)
		{
			return await _gtfsService.RegenerateSchedule(model);
		}

		[HttpDelete("trips/{tripId}")]
		[Authorize(Roles = "Administrator")]
		public async Task<ApiResponse> DeleteTrip(string tripId)
		{
			return await _gtfsService.DeleteTrip(tripId);
		}
	}
}
