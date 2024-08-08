using BlogApi.Data;
using BlogApi.Extensions;
using BlogApi.Repositories;
using BlogApi.Repositories.Base;
using BlogApi.Services;
using BlogApi.Services.Base;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BlogDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSqlDev");
    options.UseNpgsql(connectionString);
});



builder.Services.AddTransient<IBlogService, BlogService>();
builder.Services.AddTransient<IBlogRepository, BlogEfRepository>();

builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<ITopicRepository, TopicEfRepository>();



builder.Services.InitCors();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.UseCors("BlazorApp");

app.Run();

