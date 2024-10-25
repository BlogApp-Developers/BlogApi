using System.Text;
using System.Text.Json;
using BlogApi.Data;
using BlogApi.Enums;
using BlogApi.Models;
using BlogApi.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace BlogApi.HostedServices;
public class UserHostedService : IHostedService
{
    private readonly ConnectionFactory rabbitMqConnectionFactory;
    private readonly IConnection connection;
    private readonly IModel model;
    private const string QUEUE_NAME = "user";
    private readonly IDbContextFactory<BlogDbContext> dbContextFactory;
    public UserHostedService(IDbContextFactory<BlogDbContext> dbContextFactory, IOptions<RabbitMqOptions> optionsSnapshot, IConfiguration configuration)
    {
        this.rabbitMqConnectionFactory = new ConnectionFactory()
        {
            HostName = optionsSnapshot.Value.HostName,
            UserName = optionsSnapshot.Value.UserName,
            Password = optionsSnapshot.Value.Password
        };
        this.dbContextFactory = dbContextFactory;
        this.connection = this.rabbitMqConnectionFactory.CreateConnection();
        this.model = connection.CreateModel();
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        var result = this.model.QueueDeclare(
            queue: QUEUE_NAME,
            durable: true,
            exclusive: false,
            autoDelete: false
        );
        var consumer = new EventingBasicConsumer(this.model);
        consumer.Received += (sender, deliverEventArgs) =>
        {
            string? newUserJson = null;
            try {
                var biteArray = deliverEventArgs.Body.ToArray();
                newUserJson = Encoding.Unicode.GetString(biteArray);
                var newUserJsonData = newUserJson.Split('&');
                var rabbitMqAction = JsonSerializer.Deserialize<RabbitMQAction>(newUserJsonData[0])!;
                var newUser = JsonSerializer.Deserialize<User>(newUserJsonData[1])!;
                using var dbContext = this.dbContextFactory.CreateDbContext();
                
                switch (rabbitMqAction)
                {
                    case RabbitMQAction.Create:
                    {
                        dbContext.Users.Add(newUser);
                        dbContext.SaveChanges();
                        break;
                    }
                    case RabbitMQAction.Update:
                    {
                        dbContext.Users.Update(newUser);
                        dbContext.SaveChanges();
                        break;
                    }
                    case RabbitMQAction.Delete:
                    {
                        dbContext.Users.Remove(newUser);
                        dbContext.SaveChanges();
                        break;
                    }
                    default:
                    throw new ArgumentException("Invalid action!");
                }
            }
            catch(Exception ex) {
                System.Console.WriteLine($"Couldn't pull new user: '{ex}' | Body: {newUserJson}");
            }
        };
        this.model.BasicConsume(
            queue: QUEUE_NAME,
            autoAck: true,
            consumer: consumer
        );
        return Task.CompletedTask;
    }
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}