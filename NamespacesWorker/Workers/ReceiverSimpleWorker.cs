using Azure.Messaging.EventGrid.Namespaces;
using NamespacesWorker.Miscellaneous;

namespace NamespacesWorker.Workers;

public sealed class ReceiverSimpleWorker : BackgroundService
{
    private readonly ILogger<ReceiverSimpleWorker> _logger;

    private readonly EventGridReceiverClient _receiverClient;

    public ReceiverSimpleWorker(ILogger<ReceiverSimpleWorker> logger, EventGridReceiverClient receiverClient)
    {
        _logger = logger;

        // You can create it as well: new EventGridReceiverClient(endpoint, topic, subscription, new AzureKeyCredential(accessKey));
        _receiverClient = receiverClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveResult result = await _receiverClient.ReceiveAsync(maxEvents: 2, maxWaitTime: TimeSpan.FromSeconds(15), stoppingToken);

            List<string> toAcknowledge = [];

            foreach (ReceiveDetails detail in result.Details)
            {
                string lockToken = detail.BrokerProperties.LockToken;

                WeatherForecast weather = detail.Event.Data?.ToObjectFromJson<WeatherForecast>()!;

                _logger.LogInformation("Forecast will be acknowledged. Weather in {city}", weather.City);

                toAcknowledge.Add(lockToken);
            }

            AcknowledgeResult acknowledgeResult = await _receiverClient.AcknowledgeAsync(toAcknowledge, stoppingToken);

            await Task.Delay(Random.Shared.Next(2000, 3_000), stoppingToken);
        }
    }
}
