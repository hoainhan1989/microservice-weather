using cloudweather.Report.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<WeatherReportDbContext>(opt =>
{
    opt.EnableSensitiveDataLogging();
    opt.EnableDetailedErrors();
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.Run();
