using AcademiaX_Business.Abstraction;
using AcademiaX_Business.Dtos.Gtfs;
using AcademiaX_Core.Configuration;
using AcademiaX_Core.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.IO.Compression;

namespace AcademiaX_Business.Concrete
{
	public class GtfsService : IGtfsService
	{
		private readonly string zipPath;

		public GtfsService(IOptions<GtfsSettings> options)
		{
			zipPath = options.Value.DataPath; // Bu artık gtfs.zip dosyasının yolu
		}

		private IEnumerable<string> ReadLinesFromZip(string fileName)
		{
			using var zip = ZipFile.OpenRead(zipPath);
			var entry = zip.GetEntry(fileName);
			if (entry == null) throw new FileNotFoundException($"{fileName} not found in GTFS archive.");

			using var reader = new StreamReader(entry.Open());
			while (!reader.EndOfStream)
			{
				yield return reader.ReadLine();
			}
		}

		public async Task<ApiResponse> GetStops()
		{
			var response = new ApiResponse();
			try
			{
				var result = ReadLinesFromZip("stops.txt").Skip(1)
					.Select(line =>
					{
						var p = line.Split(',');
						return new StopDTO
						{
							StopId = p[0],
							StopName = p[1],
							StopLat = double.TryParse(p[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ? lat : 0,
							StopLon = double.TryParse(p[3], NumberStyles.Any, CultureInfo.InvariantCulture, out var lon) ? lon : 0,
						};
					}).ToList();

				response.Result = result;
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.ErrorMessages.Add(ex.Message);
			}

			return response;
		}

		public async Task<ApiResponse> GetTrips()
		{
			var response = new ApiResponse();
			try
			{
				var result = ReadLinesFromZip("trips.txt").Skip(1)
					.Select(line =>
					{
						var p = line.Split(',');
						return new TripDTO
						{
							TripId = p[2],
							RouteId = p[0],
							ServiceId = p[1],
							DirectionId = int.Parse(p[3])
						};
					}).ToList();

				response.Result = result;
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.ErrorMessages.Add(ex.Message);
			}

			return response;
		}

		public async Task<ApiResponse> GetStopTimes()
		{
			var response = new ApiResponse();
			try
			{
				var result = ReadLinesFromZip("stop_times.txt").Skip(1)
					.Select(line =>
					{
						var p = line.Split(',');
						var arrival = TimeSpan.Parse(p[1]);
						var departure = arrival.Add(TimeSpan.FromMinutes(5));

						return new StopTimeDTO
						{
							TripId = p[0],
							ArrivalTime = arrival.ToString(@"hh\:mm\:ss"),
							DepartureTime = departure.ToString(@"hh\:mm\:ss"),
							StopId = p[3],
							StopSequence = int.Parse(p[4])
						};
					}).ToList();

				response.Result = result;
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.ErrorMessages.Add(ex.Message);
			}

			return response;
		}

		public async Task<ApiResponse> GetStopTimeTable(string stopId, int directionId = 0)
		{
			var response = new ApiResponse();
			try
			{
				var trips = ReadLinesFromZip("trips.txt").Skip(1)
					.Select(l => l.Split(',')).Where(p => int.Parse(p[3]) == directionId).Select(p => p[2]).ToHashSet();

				var stopTimes = ReadLinesFromZip("stop_times.txt").Skip(1)
					.Select(p => p.Split(','))
					.Where(p => p[3] == stopId && trips.Contains(p[0]))
					.Select(p =>
					{
						var arrival = TimeSpan.Parse(p[1]);
						var departure = arrival.Add(TimeSpan.FromMinutes(5));
						return new StopTimeDTO
						{
							TripId = p[0],
							ArrivalTime = arrival.ToString(@"hh\:mm\:ss"),
							DepartureTime = departure.ToString(@"hh\:mm\:ss"),
							StopId = p[3],
							StopSequence = int.Parse(p[4])
						};
					})
					.OrderBy(p => p.ArrivalTime).ToList();

				response.Result = stopTimes;
				response.IsSuccess = true;
				response.StatusCode = System.Net.HttpStatusCode.OK;
			}
			catch (Exception ex)
			{
				response.IsSuccess = false;
				response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
				response.ErrorMessages.Add(ex.Message);
			}

			return response;
		}
	}
}
