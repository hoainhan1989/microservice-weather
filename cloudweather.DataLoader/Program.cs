using cloudweather.DataLoader.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

IConfiguration config = new ConfigurationBuilder().AddJsonFile("appsettings.json")
    .AddEnvironmentVariables().Build();

var servicesConfig = config.GetSection("Services");

var tempServiceConfig = servicesConfig.GetSection("Temperature");
var tempHost = tempServiceConfig["Host"];
var tempPort = tempServiceConfig["port"];

var precipServiceConfig = servicesConfig.GetSection("Precipitation");
var precipHost = precipServiceConfig["Host"];
var precipPort = precipServiceConfig["port"];

var zipCodes = new List<string> {
    "73026",
    "68014",
    "04401",
    "32808",
    "19717"
};

Console.WriteLine("Starting Data Load");
var temperatureClient = new HttpClient();
temperatureClient.BaseAddress = new Uri($"http://{tempHost}:{tempPort}");

var precipitationClient = new HttpClient();
precipitationClient.BaseAddress = new Uri($"http://{precipHost}:{precipPort}");

foreach (var zip in zipCodes)
{
    Console.WriteLine($"Processing Zip Code: {zip}");
    var from = DateTime.Now.AddYears(-2);
    var thru = DateTime.Now;

    for(var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
    {
        var temps = PostTemp(zip, day, temperatureClient);
        PostPrecip(temps[0], zip, day, precipitationClient);
    }

}

void PostPrecip(object value, string zip, DateTime day, HttpClient precipitationClient)
{
    var precipitation = new PrecipitationModel
    {
        AmountInches = 12,
        WeatherType = "rain",
        ZipCode = zip,
        CreatedOn = day
    };

    var precipResponse = precipitationClient.PostAsJsonAsync("observation", precipitation).Result;

    if(precipResponse.IsSuccessStatusCode)
    {
        Console.Write($"Body: {JsonConvert.SerializeObject(precipitation)}");
    }
}

List<int> PostTemp(string zip, DateTime day, HttpClient temperatureClient)
{
    var temperatureObservation = new TemperationModel
    {
        TempHighF = 10,
        TempLowF = 2,
        ZipCode = zip,
        CreatedOn = day
    };
    var tempResponse = temperatureClient.PostAsJsonAsync("observation", temperatureObservation).Result;

    if (tempResponse.IsSuccessStatusCode)
    {
        Console.Write($"Body: {JsonConvert.SerializeObject(temperatureObservation)}");
    }

    return new List<int>() { 10,2};
}