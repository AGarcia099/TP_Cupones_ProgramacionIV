using Microsoft.EntityFrameworkCore;
using CuponesApiTp.Data;
using System.Data.Common;
using System.Text.Json.Serialization;
using CuponesApiTp.Interfaces;
using CuponesApiTp.Services;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (WindowsServiceHelpers.IsWindowsService())
{
    builder.Host.UseWindowsService();
    builder.WebHost.UseContentRoot(AppContext.BaseDirectory);
}

builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbContext")));

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Logger(l => 
        l.Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Error)
        .WriteTo.File("Logs/Log-Error-.txt", rollingInterval: RollingInterval.Day)
    )
    .WriteTo.Logger(l =>
        l.Filter.ByIncludingOnly(evt => evt.Level == Serilog.Events.LogEventLevel.Information)
        .WriteTo.File("Logs/Log-.txt", rollingInterval: RollingInterval.Day)
    )
    .CreateLogger();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddScoped<ICuponesServices, CuponesServices>();
builder.Services.AddScoped<ISendEmailService, SendEmailService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
