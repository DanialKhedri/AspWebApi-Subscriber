#region Usings
using Application.DTOs.Data;
using Domain.Entities;
using infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Linq;
using System.Text;
#endregion

namespace MyProject.Infrastructure.RabbitMQ;

public class RabbitMQMessageBroker : IHostedService
{
    #region Ctor
    private readonly string _hostname = "localhost";
    private readonly string _queueName = "dataQueue";
    private const int _minuteInterval = 60 * 1000;

    private readonly ILogger<RabbitMQMessageBroker> _logger;
    private IConnection _connection;
    private IChannel _channel;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    private AsyncEventingBasicConsumer _consumer;
    private readonly List<DataPointPublishDTO> _dataBuffer = new List<DataPointPublishDTO>();
    private Timer _timer;


    public RabbitMQMessageBroker(ILogger<RabbitMQMessageBroker> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    #endregion


    // متد برای شروع RabbitMQ
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

            // تنظیم تایمر برای ذخیره داده‌ها هر یک دقیقه
            _timer = new Timer(SaveDataToDatabase, null, _minuteInterval, _minuteInterval);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error starting RabbitMQ service: {ex.Message}");
        }
    }


    // متد برای توقف RabbitMQ
    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            _logger.LogInformation("RabbitMQ Service stopped.");

            // متوقف کردن تایمر هنگام توقف سرویس
            _timer?.Dispose();
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
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
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
                // افزودن داده‌ها به بافر
                lock (_dataBuffer)
                {
                    _dataBuffer.AddRange(dataPoints);
                }
                _logger.LogInformation($"Added {dataPoints.Count} data points to buffer.");
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


    // متد برای ذخیره‌سازی داده‌ها در پایگاه‌داده هر یک دقیقه
    private async void SaveDataToDatabase(object state)
    {
        try
        {
            List<DataPointPublishDTO> dataToSave = null;

            // قفل کردن بافر داده‌ها و کپی کردن آن برای ذخیره در پایگاه‌داده
            lock (_dataBuffer)
            {

                // Get the last 10 items from the buffer
                dataToSave = new List<DataPointPublishDTO>(_dataBuffer).TakeLast(10).ToList();

                _dataBuffer.Clear();
            }

            if (dataToSave != null && dataToSave.Any())
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<DataContext>();

                    // ایجاد یک رکورد جدید برای ذخیره‌سازی در دیتابیس
                    var newDataRecord = new DataRecord() // فرض کنید DataRecord کلاس برای ذخیره چند DataPoint است
                    {
                        Time = dataToSave[4].Time,

                        DataPoint1Value = dataToSave[0].Value,
                        DataPoint2Value = dataToSave[1].Value,
                        DataPoint3Value = dataToSave[2].Value,
                        DataPoint4Value = dataToSave[3].Value,
                        DataPoint5Value = dataToSave[4].Value,
                        DataPoint6Value = dataToSave[5].Value,
                        DataPoint7Value = dataToSave[6].Value,
                        DataPoint8Value = dataToSave[7].Value,
                        DataPoint9Value = dataToSave[8].Value,
                        DataPoint10Value = dataToSave[9].Value,

                    };



                    var recordisexist = await context.DataRecords.FirstOrDefaultAsync(d => d.Time == newDataRecord.Time);

                    if (recordisexist != null)
                    {

                        recordisexist.DataPoint1Value = dataToSave[0].Value;
                        recordisexist.DataPoint2Value = dataToSave[1].Value;
                        recordisexist.DataPoint3Value = dataToSave[2].Value;
                        recordisexist.DataPoint4Value = dataToSave[3].Value;
                        recordisexist.DataPoint5Value = dataToSave[4].Value;
                        recordisexist.DataPoint6Value = dataToSave[5].Value;
                        recordisexist.DataPoint7Value = dataToSave[6].Value;
                        recordisexist.DataPoint8Value = dataToSave[7].Value;
                        recordisexist.DataPoint9Value = dataToSave[8].Value;
                        recordisexist.DataPoint10Value = dataToSave[9].Value;


                        context.DataRecords.Update(recordisexist);


                    }
                    else 
                    {
                        await context.DataRecords.AddAsync(newDataRecord);
                    }

         
                    _logger.LogInformation($"Data record saved to database at {newDataRecord.Time}");
                    await context.SaveChangesAsync();
                }
            }
            else
            {
                _logger.LogInformation("No new data to save to database.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving data to database: {ex.Message}");
        }
    }


}
