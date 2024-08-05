using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text.Json;

namespace ConsoleAppMQTT;

public sealed record Message(string Content);

public static class MqttClientExample
{
    public static async Task Run(CancellationToken cancelToken)
    {
        // --> Configuration
        MqttClientOptions clientOptions = Settings.GetMqttClientOptions();

        var mqttFactory = new MqttFactory();

        // --> Create client
        using IMqttClient client = mqttFactory.CreateMqttClient();

        // --> Connect with client
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
        string topic = arg.ApplicationMessage.Topic;
        //string payload = arg.ApplicationMessage.ConvertPayloadToString();

        Message? messageObject = JsonSerializer.Deserialize<Message>(arg.ApplicationMessage.PayloadSegment);

        await Console.Out.WriteLineAsync($"'{topic}' - Payload: '{messageObject}'");

        // await arg.AcknowledgeAsync(CancellationToken.None); // Not sure about it
    }

    private static async Task publisher(IMqttClient client, CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            MqttApplicationMessage message = createMessage();

            MqttClientPublishResult publishResult = await client.PublishAsync(message);

            await Task.WhenAny(Task.Delay(2_000, cancelToken)); // No TaskCanceledException
        }
    }

    private static MqttApplicationMessage createMessage()
    {
        string payload = $"Current time: {DateTime.Now.ToLongTimeString()}";

        var messageObject = new Message(payload);

        byte[] messageBytes = JsonSerializer.SerializeToUtf8Bytes(messageObject);

        MqttApplicationMessage message = new MqttApplicationMessageBuilder()
            .WithTopic(Settings.Topic)
            .WithPayload(messageBytes)
            //.WithPayload(payload) // You can simply send a Payload as string
            .Build();

        return message;
    }
}
