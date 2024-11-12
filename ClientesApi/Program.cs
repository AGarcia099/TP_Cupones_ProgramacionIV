using ClientesApi.Data;
using ClientesApi.Interfaces;
using ClientesApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<DataBaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("dbContext")));

//Establecer el puerto que fue especificado en appsettings.json
var puertoApi = builder.Configuration.GetValue<int>("Configuracion:PuertoApi");
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(puertoApi);
//});

builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddHttpClient();
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
