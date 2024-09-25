using Microsoft.EntityFrameworkCore;
using PermissionsAPI.CQRS.Commands;
using PermissionsAPI.CQRS.Queries;
using PermissionsAPI.Data;
using PermissionsAPI.ElasticSearch.Interfaces;
using PermissionsAPI.ElasticSearch;
using PermissionsAPI.Kafka;
using PermissionsAPI.Kafka.Interfaces;
using PermissionsAPI.Models;
using PermissionsAPI.Repositories;
using PermissionsAPI.Repositories.Permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using PermissionsAPI.Common;

var builder = WebApplication.CreateBuilder(args);
var MyAllowPolicy = "AllowAllOrigins";

// Configurar Elasticsearch Settings
builder.Services.Configure<ElasticsearchSettings>(builder.Configuration.GetSection("ElasticsearchSettings"));

// Registrar el servicio de Elasticsearch
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();

// Register KafkaSetting with IOptions pattern
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("KafkaSettings"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inject Database
var password = Environment.GetEnvironmentVariable("MSSQL_SA_PASSWORD");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
connectionString = string.Format(connectionString, password);

builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(connectionString));

// REGISTERS

//Kafka Producer
builder.Services.AddScoped<IKafkaProducer, KafkaProducer>();

// Repositorios
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRepository<PermissionType>, Repository<PermissionType>>();

// Commands
builder.Services.AddScoped<CommandHandler>();

// Queries
builder.Services.AddScoped<QueryHandler>();

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(QueryHandler).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(typeof(PermissionType));

// Allows CORS origins
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: MyAllowPolicy,
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error is NotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new
            {
                message = exceptionHandlerPathFeature.Error.Message
            });
        }
    });
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware setup, CORS, controllers, etc.
app.UseCors(MyAllowPolicy);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
