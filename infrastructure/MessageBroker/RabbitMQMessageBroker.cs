using Application.DTOs.Data;
using Domain.Entities;
using Domain.Interfaces.IRepository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json; // برای تبدیل JSON به شیء

namespace MyProject.Infrastructure.RabbitMQ;



public class RabbitMQMessageBroker : IHostedService
{
    private readonly string _hostname = "localhost";  // آدرس RabbitMQ (نام سرویس RabbitMQ در Docker Compose)
    private readonly string _queueName = "dataQueue"; // نام صف
    private readonly ILogger<RabbitMQMessageBroker> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private AsyncEventingBasicConsumer _consumer;



    public RabbitMQMessageBroker(ILogger<RabbitMQMessageBroker> logger)
    { 
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            // تلاش برای اتصال به RabbitMQ
            _logger.LogInformation("Starting RabbitMQ connection...");
            await ConnectToRabbitMQ();

            // بررسی اینکه آیا کانال به درستی مقداردهی شده است یا خیر
            if (_channel == null)
            {
                _logger.LogError("Channel is null. Cannot start consuming messages.");
                return;
            }

            // ایجاد consumer برای دریافت پیام‌ها
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                _logger.LogInformation("Message received from RabbitMQ.");
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                await ProcessMessageAsync(message, cancellationToken);
            };

            // شروع به دریافت پیام‌ها از صف
            await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: _consumer);
            _logger.LogInformation("RabbitMQ Service started and listening for messages.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting RabbitMQ service: {ex.Message}");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            _logger.LogInformation("RabbitMQ Service stopped.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error stopping RabbitMQ service: {ex.Message}");
        }
        return Task.CompletedTask;
    }

    // متد برای اتصال به RabbitMQ با استفاده از retry logic
    private async Task ConnectToRabbitMQ()
    {
        // اتصال به RabbitMQ
        var factory = new ConnectionFactory()
        {
            HostName = _hostname,
            Port = 5672,
        };

        int retryCount = 5;

        while (retryCount > 0)
        {
            try
            {
                _logger.LogInformation("Attempting to connect to RabbitMQ...");
                _connection = await factory.CreateConnectionAsync();
                _logger.LogInformation("Connection to RabbitMQ established.");

                // ایجاد کانال
                _channel = await _connection.CreateChannelAsync();
                _logger.LogInformation("Channel created successfully.");

                // اعلام صف
                await _channel.QueueDeclareAsync(
                    queue: _queueName,
                    durable: false,      // صف دایمی نباشد
                    exclusive: false,    // صف اختصاصی نباشد
                    autoDelete: false,   // صف حذف خودکار نشود
                    arguments: null);    // هیچ آرگومانی برای صف نداریم
                _logger.LogInformation("Queue declared successfully.");

                return; // موفقیت‌آمیز
            }
            catch (Exception ex)
            {
                retryCount--;
                _logger.LogError($"Failed to connect to RabbitMQ. Retries left: {retryCount}. Error: {ex.Message}");
                if (retryCount == 0) throw;
                await Task.Delay(5000); // Retry after 5 seconds
            }
        }
    }

    // متد برای پردازش پیام‌های دریافتی
    private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received message: {message}");

        try
        {
            // تبدیل پیام JSON به لیستی از DataPoint
            var dataPoints = JsonConvert.DeserializeObject<List<DataPointPublishDTO>>(message);

            if (dataPoints != null && dataPoints.Any())
            {
                foreach (var dataPoint in dataPoints)
                {
                    _logger.LogInformation($"Processing DataPoint: Name={dataPoint.Name}, Value={dataPoint.Value}, Time={dataPoint.Time}");

                    // عملیات ذخیره‌سازی یا هر پردازش دیگری که نیاز دارید در اینجا انجام دهید

                    DataPoint dataPoint1 = new DataPoint()
                    {
                        Name = dataPoint.Name,
                        Time = dataPoint.Time,
                        Value = dataPoint.Value,
                    };
                    //await _dataPointRepository.AddDatapointAsync(dataPoint1);

                }

                //await _dataPointRepository.SaveChangeAsync();
            }
            else
            {
                _logger.LogWarning("Received an empty or invalid message.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error processing message: {ex.Message}");
        }
    }
}



