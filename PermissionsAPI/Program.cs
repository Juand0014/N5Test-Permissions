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

var builder = WebApplication.CreateBuilder(args);
var MyAllowPolicy = "Permission";

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
builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// Allows CORS origins
builder.Services.AddCors(option =>
{
    option.AddPolicy(name: MyAllowPolicy,
        policy =>
        {
            policy.AllowAnyOrigin();
            policy.AllowAnyHeader();
            policy.AllowAnyMethod();
        });
});

builder.Services.AddControllers();

var app = builder.Build();

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
