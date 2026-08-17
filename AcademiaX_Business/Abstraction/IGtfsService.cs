using AcademiaX_Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademiaX_Business.Abstraction
{
	public interface IGtfsService
	{
		Task<ApiResponse> GetStops();
		Task<ApiResponse> GetTrips();
		Task<ApiResponse> GetStopTimes();
		Task<ApiResponse> GetStopTimeTable(string stopId, int directionId = 0);

		/// İki durak arasındaki bir sonraki seferi (yön, kalkış/varış saati, süre) bulur.
		Task<ApiResponse> GetRoutePlan(string fromStopId, string toStopId, string afterTime = null);
	}
}
