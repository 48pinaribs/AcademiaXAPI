using AcademiaX_Business.Abstraction;
using AcademiaX_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AcademiaX.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
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
	}
}
