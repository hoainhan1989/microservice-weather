using cloudweather.Temperature.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TemperatureDbContext>(opt =>
{
    opt.EnableSensitiveDataLogging();
    opt.EnableDetailedErrors();
    opt.UseNpgsql(builder.Configuration.GetConnectionString("AppDb"));
}, ServiceLifetime.Transient);

var app = builder.Build();

app.Run();
