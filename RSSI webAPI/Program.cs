using RSSI_webAPI.Repositories.Contracts;
using RSSI_webAPI.Repositories;
using RSSI_webAPI.Authorization;
using RSSI_webAPI.Extensions;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using RSSI_webAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services


builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen(def => {
    def.AddSecurityDefinition("StaticApiKey", new OpenApiSecurityScheme
    {
        Description = "The Api key to access the controllers",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "StaticApiKeyAuthorizationScheme",
    });

    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Id = "StaticApiKey",
            Type = ReferenceType.SecurityScheme,
        },
        In = ParameterLocation.Header,
    };

    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };

    def.AddSecurityRequirement(requirement);
});

builder.Services.AddScoped<ISatelliteDataRepository, SatelliteDataRepository>();
builder.Services.AddScoped<IEarthDataRepository, EarthDataRepository>();
builder.Services.AddAutoMapper(typeof(MappingConfiguration));
builder.Services.AddScoped<AuthFilter>();

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

// app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
