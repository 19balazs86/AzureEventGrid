using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;

namespace ConsoleAppMQTT;

public static class MqttClientExample
{
    public static async Task Run(CancellationToken cancelToken)
    {
        MqttClientOptions clientOptions = Settings.GetMqttClientOptions();

        var mqttFactory = new MqttFactory();

        using IMqttClient client = mqttFactory.CreateMqttClient();

        MqttClientConnectResult connectResult = await client.ConnectAsync(clientOptions);

        Console.WriteLine($"ConnectResult: {connectResult.ResultCode}");

        // --> Start: Publisher
        Task publisherTask = publisher(client, cancelToken);

        // --> Subscribe to topic
        await client.SubscribeAsync(Settings.Topic);

        client.ApplicationMessageReceivedAsync += client_MessageReceived;

        // --> Wait for the publisher until cancelToken closed
        await publisherTask;

        // --> Disconnect
        // Send a clean disconnect to the server by calling the DisconnectAsync(...)
        // Without disconnect, the TCP connection gets dropped and the server will handle this as a non clean disconnect
        await client.DisconnectAsync();
    }

    private static async Task client_MessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        string topic   = arg.ApplicationMessage.Topic;
        string payload = arg.ApplicationMessage.ConvertPayloadToString();

        await Console.Out.WriteLineAsync($"'{topic}' - Payload: '{payload}'");

        // await arg.AcknowledgeAsync(CancellationToken.None); // Not sure about it
    }

    private static async Task publisher(IMqttClient client, CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            string payload = $"Current time: {DateTime.Now.ToLongTimeString()}";

            MqttApplicationMessage message = new MqttApplicationMessageBuilder()
                .WithTopic(Settings.Topic)
                .WithPayload(payload)
                .Build();

            MqttClientPublishResult publishResult = await client.PublishAsync(message);

            await Task.WhenAny(Task.Delay(2_000, cancelToken)); // No TaskCanceledException
        }
    }
}
