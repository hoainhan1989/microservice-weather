using cloudweather.Report.BusinessLogic;
using cloudweather.Report.Config;
using cloudweather.Report.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WeatherReportDbContext>(opt =>
{
    opt.EnableSensitiveDataLogging();
    opt.EnableDetailedErrors();
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

builder.Services.AddHttpClient();
builder.Services.AddTransient<IWeatherReportAggrator, WeatherReportAggrator>();
builder.Services.AddOptions();
builder.Services.Configure<WeatherDataConfig>(builder.Configuration.GetSection("WeatherDataConfig")); ;

var app = builder.Build();

app.MapGet("/weather-report/{zip}",async (string zip, [FromQuery] int? days, IWeatherReportAggrator weatherReport) => {
    if (days == null)
        days = 0;

    var report = await weatherReport.BuildWeeklyReport(zip, days.Value);
    return Results.Ok(report);
});

app.Run();
