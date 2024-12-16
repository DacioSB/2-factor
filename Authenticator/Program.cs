using Authenticator;
using Authenticator.Repositories;
using Authenticator.Services;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Cors;
using System.Net.Mime;
using Util;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add MongoDB configuration
var mongoSettings = builder.Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
{
    return new MongoClient(mongoSettings.ConnectionString);
});
//injection of IUserService and IUserRepository
builder.Services.AddScoped<IUserConnectionService, UserConnectionService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped(serviceProvider =>
{
    var mongoClient = serviceProvider.GetService<IMongoClient>();

    //mongoClient cannot be null
    if (mongoClient == null)
    {
        throw new ArgumentNullException(nameof(mongoClient));
    }

    return mongoClient.GetDatabase(mongoSettings.DatabaseName);
});

// Add Swagger configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API Name", Version = "v1" });
});

builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.WriteTo.Console().Enrich.FromLogContext();
});

var app = builder.Build();

app.Use(async (context, next) =>
{
    if (context.Request.ContentType?.Equals(MediaTypeNames.Application.Json) == true)
    {
        context.Request.Headers["Accept"] = MediaTypeNames.Application.Json;
    }

    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
    
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API Name v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

