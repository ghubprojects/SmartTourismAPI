using Microsoft.AspNetCore.Authentication.JwtBearer;
using SmartTourismAPI.Application;
using SmartTourismAPI.Infrastructure;
using SmartTourismAPI.Infrastructure.Extensions;
using SmartTourismAPI.WebApi;
using SmartTourismAPI.WebApi.ExceptionHandlers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddWebApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowAllOrigins", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Swagger"));
}

app.UseExceptionHandler();

app.UseStaticFiles();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Seeding data
await app.SeedDatabaseAsync().ConfigureAwait(false);

app.Run();
