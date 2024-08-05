using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;

namespace ConsoleAppMQTT;

public static class MqttManagedClientExample
{
    public static async Task Run(CancellationToken cancelToken)
    {
        MqttClientOptions clientOptions = Settings.GetMqttClientOptions();

        ManagedMqttClientOptions managedClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        var mqttFactory = new MqttFactory();

        using IManagedMqttClient client = mqttFactory.CreateManagedMqttClient();

        await client.StartAsync(managedClientOptions);

        Console.WriteLine("Connect started");

        // --> Start: Publisher
        Task publisherTask = publisher(client, cancelToken);

        // --> Subscribe to topic
        await client.SubscribeAsync(Settings.Topic);

        client.ApplicationMessageReceivedAsync += client_MessageReceived;

        // --> Wait for the publisher until cancelToken closed
        await publisherTask;

        // Before stop the client, make sure no pending messages
        SpinWait.SpinUntil(() => client.PendingApplicationMessagesCount == 0, TimeSpan.FromSeconds(5));

        await client.StopAsync();
    }

    private static async Task client_MessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        string topic   = arg.ApplicationMessage.Topic;
        string payload = arg.ApplicationMessage.ConvertPayloadToString();

        await Console.Out.WriteLineAsync($"'{topic}' - Payload: '{payload}'");

        // await arg.AcknowledgeAsync(CancellationToken.None); // Not sure about it
    }

    private static async Task publisher(IManagedMqttClient client, CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            string payload = $"Current time: {DateTime.Now.ToLongTimeString()}";

            await client.EnqueueAsync(Settings.Topic, payload);

            await Task.WhenAny(Task.Delay(2_000, cancelToken)); // No TaskCanceledException
        }
    }
}
