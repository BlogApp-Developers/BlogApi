using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Storage.Blobs;
using BlogApi.Data;
using BlogApi.Extensions;
using BlogApi.Options;
using BlogApi.Repositories;
using BlogApi.Repositories.Base;
using BlogApi.Services;
using BlogApi.Services.Base;
using BlogApi.TokenValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using BlogApi.HostedServices;

var builder = WebApplication.CreateBuilder(args);
builder.Services.InitCors();
builder.Services.InitAspnetIdentity(builder.Configuration);
builder.Services.InitAuth(builder.Configuration);
var rabbitMqSection = builder.Configuration.GetSection("RabbitMq");
builder.Services.Configure<RabbitMqOptions>(rabbitMqSection);

builder.Services.AddHostedService<UserHostedService>();


builder.Services.AddDbContext<BlogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSqlDev");
    options.UseNpgsql(connectionString);
});


// string azureBlobStorageConnectionString = builder.Configuration.GetConnectionString("AzureBlobStorage");

// // // Use the connection string as needed, for example to create a BlobServiceClient
// var blobServiceClient = new BlobServiceClient(azureBlobStorageConnectionString);

// // Add services to the container.
// builder.Services.AddSingleton(blobServiceClient);

//var connection = builder.Configuration.GetSection("Connections").GetSection("StringConnection");
//builder.Services.Configure<BlobOptions>(azureBlobStorageConnectionString);

//builder.Services.AddSingleton<BlobServiceClient>();

builder.Services.AddTransient<IBlogService, BlogService>();
builder.Services.AddTransient<IBlogRepository, BlogEfRepository>();

builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<ITopicRepository, TopicEfRepository>();
builder.Services.AddTransient<TokenValidation>();

// builder.Services.AddControllers()
//     .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
//     });

// builder.Services.AddControllers()
//     .AddNewtonsoftJson(options =>
//     {
//         options.SerializerSettings.ContractResolver = new DefaultContractResolver();
//         options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
//     });


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });



builder.Services.AddAuthorization();
//builder.Services.AddAuthentication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseCors("BlazorApp");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();

