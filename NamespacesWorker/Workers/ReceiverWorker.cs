using Azure;
using Azure.Messaging.EventGrid.Namespaces;
using NamespacesWorker.Miscellaneous;

namespace NamespacesWorker.Workers;

public sealed class ReceiverWorker : BackgroundService
{
    private readonly ILogger<ReceiverWorker> _logger;

    private readonly EventGridReceiverClient _receiverClient;

    public ReceiverWorker(ILogger<ReceiverWorker> logger, EventGridReceiverClient receiverClient)
    {
        _logger = logger;

        // You can create it as well: new EventGridReceiverClient(endpoint, topic, subscription, new AzureKeyCredential(accessKey));
        _receiverClient = receiverClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            ReceiveResult result = await _receiverClient.ReceiveAsync(maxEvents: 3, maxWaitTime: TimeSpan.FromSeconds(15), stoppingToken);

            List<string> toAcknowledge = []; // Acknowledge it, thereby deleting it from the subscription
            List<string> toRelease     = []; // Release it, thereby allowing other consumers to receive it
            List<string> toReject      = []; // Reject it, events will be moved to the dead letter queue if it is configured

            foreach (ReceiveDetails detail in result.Details)
            {
                string lockToken = detail.BrokerProperties.LockToken;

                WeatherForecast weather = detail.Event.Data?.ToObjectFromJson<WeatherForecast>()!;

                if (Random.Shared.NextDouble() <= 0.1)
                {
                    if (Random.Shared.NextDouble() <= 0.4)
                    {
                        _logger.LogInformation("Forecast will be rejected. Weather in {city}", weather.City);

                        toReject.Add(lockToken);
                    }
                    else
                    {
                        _logger.LogInformation("Forecast will be released. Weather in {city}", weather.City);

                        toRelease.Add(lockToken);
                    }

                    continue;
                }

                _logger.LogInformation("Forecast will be acknowledged. Weather in {city}", weather.City);

                toAcknowledge.Add(lockToken);
            }

            // -> Acknowledge
            if (toAcknowledge.Count > 0)
            {
                _logger.LogInformation("Acknowledge events");

                AcknowledgeResult acknowledgeResult = await _receiverClient.AcknowledgeAsync(toAcknowledge, stoppingToken);

                handleResult(acknowledgeResult.SucceededLockTokens.Count, acknowledgeResult.FailedLockTokens);
            }

            // -> Release
            if (toRelease.Count > 0)
            {
                _logger.LogInformation("Release events");

                ReleaseResult releaseResult = await _receiverClient.ReleaseAsync(toRelease, cancellationToken: stoppingToken);

                handleResult(releaseResult.SucceededLockTokens.Count, releaseResult.FailedLockTokens);
            }

            // -> Reject
            if (toReject.Count > 0)
            {
                _logger.LogInformation("Reject events");

                RejectResult rejectResult = await _receiverClient.RejectAsync(toReject, stoppingToken);

                handleResult(rejectResult.SucceededLockTokens.Count, rejectResult.FailedLockTokens);
            }

            await Task.Delay(Random.Shared.Next(500, 1_000), stoppingToken);
        }
    }

    private void handleResult(int succeededCount, IReadOnlyList<FailedLockToken> failedLockTokens)
    {
        _logger.LogInformation("Success count: {count}", succeededCount);

        if (failedLockTokens.Any())
        {
            _logger.LogError("Failed count: {count}", failedLockTokens.Count);

            foreach (FailedLockToken failedLockToken in failedLockTokens)
            {
                ResponseError error = failedLockToken.Error;

                _logger.LogError("Error Code: '{code}', Description: '{message}'", error.Code, error.Message);
            }
        }
    }
}
