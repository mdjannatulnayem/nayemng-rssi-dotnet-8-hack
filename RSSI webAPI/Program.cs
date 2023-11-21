using RSSI_webAPI.Repositories.Contracts;
using RSSI_webAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IEarthDataRepository, EarthDataRepository>();

// Configure CORS policy
builder.Services.AddCors(p => p.AddPolicy("corspolicy", build => {
    build.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("corspolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
