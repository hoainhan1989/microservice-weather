using cloudweather.Report.Config;
using cloudweather.Report.DataAccess;
using cloudweather.Report.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace cloudweather.Report.BusinessLogic
{
    public interface IWeatherReportAggrator
    {
        public Task<WeatherReport> BuildWeeklyReport(string zip, int days);
    }
    public class WeatherReportAggrator: IWeatherReportAggrator
    {
        private readonly IHttpClientFactory _http;
        private readonly ILogger<WeatherReportAggrator> _logger;
        private readonly WeatherReportDbContext _db;
        private readonly WeatherDataConfig _weatherDataConfig;
        public WeatherReportAggrator(IHttpClientFactory http, ILogger<WeatherReportAggrator> logger, WeatherReportDbContext db, 
            IOptions<WeatherDataConfig> weatherDataConfig)
        {
            _http = http;
            _logger = logger;
            _db = db;
            _weatherDataConfig = weatherDataConfig.Value;
        }

        public async Task<WeatherReport> BuildWeeklyReport(string zip, int days)
        {
            var httpClient = _http.CreateClient();
            var precipData = await FetchPrecipitationData(httpClient, zip, days);
            var totalSnow = GetTotalSnow(precipData);
            var totalRain = GetTotalRain(precipData);

            _logger.LogInformation($"BuildWeeklyReport log total snow {totalSnow}");

            var tempData = await FetchTemperatureData(httpClient, zip, days);
            var averageHighTemp = tempData.Average(t=> t.TempHighF);
            var averageLowTemp = tempData.Average(t => t.TempLowF);

            var weeklyWeatherReport = new WeatherReport
            {
                AverageHighF = Math.Round(averageHighTemp, 1),
                AverageLowF = Math.Round(averageLowTemp, 1),
                RainfallTotalInches = totalRain,
                SnowTotalInches = totalSnow,
                ZipCode = zip,
                CreatedOn = DateTime.UtcNow
            };

            _db.Add(weeklyWeatherReport);
            await _db.SaveChangesAsync();

            return weeklyWeatherReport;
        }

        private decimal GetTotalRain(List<PrecipitationModel> precipData)
        {
            var totalRain = precipData.Where(p => p.WeatherType == "rain")
                .Sum(p=> p.AmountInches);

            return Math.Round(totalRain, 1);
        }

        private decimal GetTotalSnow(List<PrecipitationModel> precipData)
        {
            var totalRain = precipData.Where(p => p.WeatherType == "snow")
                .Sum(p => p.AmountInches);

            return Math.Round(totalRain, 1);
        }

        private async Task<List<TemperatureModel>> FetchTemperatureData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildTempeartureServiceEndpoint(zip, days);
            var tempRecords = await httpClient.GetAsync(endpoint);
            var tempData = await tempRecords.Content.ReadFromJsonAsync<List<TemperatureModel>>();
            return tempData ?? new List<TemperatureModel>();
        }

        private string BuildTempeartureServiceEndpoint(string zip, int days)
        {
            var tempServiceProtocol = _weatherDataConfig.TempDataProtocol;
            var tempServiceHost = _weatherDataConfig.TempDataHost;
            var tempServicePort = _weatherDataConfig.TempDataPort;

            return $"{tempServiceProtocol}://{tempServiceHost}:{tempServicePort}/observation/{zip}?day={days}";

        }

        private async Task<List<PrecipitationModel>> FetchPrecipitationData(HttpClient httpClient, string zip, int days)
        {
            var endpoint = BuildPrecipitationServiceEndpoint(zip, days);
            var precipRecords = await httpClient.GetAsync(endpoint);
            var jsonSerializeOptions = new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var precipData = await precipRecords.Content.ReadFromJsonAsync<List<PrecipitationModel>>(jsonSerializeOptions);
            return precipData ?? new List<PrecipitationModel>();
        }

        private string BuildPrecipitationServiceEndpoint(string zip, int days)
        {
            var precipServiceProtocol = _weatherDataConfig.PrecipDataProtocol;
            var precipServiceHost = _weatherDataConfig.PrecipDataHost;
            var precipServicePort = _weatherDataConfig.PrecipDataPort;

            return $"{precipServiceProtocol}://{precipServiceHost}:{precipServicePort}/observation/{zip}?day={days}";
        }
    }
}
