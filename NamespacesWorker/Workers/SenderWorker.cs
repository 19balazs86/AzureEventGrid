using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid.Namespaces;
using NamespacesWorker.Miscellaneous;

namespace NamespacesWorker.Workers;

public sealed class SenderWorker : BackgroundService
{
    private readonly ILogger<SenderWorker> _logger;

    private readonly EventGridSenderClient _senderClient;

    public SenderWorker(ILogger<SenderWorker> logger, EventGridSenderClient senderClient)
    {
        _logger = logger;

        // You can create it as well: new EventGridSenderClient(endpoint, topic, new AzureKeyCredential(accessKey));
        _senderClient = senderClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            CloudEvent[] cloudEvents = createCloudEvents(5).ToArray();

            Response response = await _senderClient.SendAsync(cloudEvents, stoppingToken);

            _logger.LogInformation("Events are sent with status: {status}", response.Status);

            await Task.Delay(Random.Shared.Next(500, 2_000), stoppingToken);
        }
    }

    private static IEnumerable<CloudEvent> createCloudEvents(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var weatherForecast = WeatherForecast.Create();

            yield return new CloudEvent("Weather-Source", "Forecast-Type", weatherForecast);
        }
    }
}
