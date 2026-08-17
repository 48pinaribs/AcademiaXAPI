using AcademiaX_Business.Dtos.Gtfs;
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

		// --- Admin: ring yönetimi (bkz. GtfsController — bu üçü [Authorize(Roles="Administrator")]) ---

		/// Yeni durak ekler ya da var olan bir durağı (isim/konum) günceller.
		Task<ApiResponse> UpsertStop(UpsertStopRequestDTO model);

		/// Bir durağı ve ona ait tüm StopTime kayıtlarını siler.
		Task<ApiResponse> DeleteStop(string stopId);

		/// Var olan tüm Trip/StopTime kayıtlarını silip verilen parametrelerle yeniden oluşturur.
		Task<ApiResponse> RegenerateSchedule(RegenerateScheduleRequestDTO model);

		/// Tek bir seferi (Trip + StopTime'ları) iptal eder.
		Task<ApiResponse> DeleteTrip(string tripId);
	}
}
