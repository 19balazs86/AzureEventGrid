using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using System.Text.Json;

namespace ConsoleAppMQTT;

public static class MqttManagedClientExample
{
    public static async Task Run(CancellationToken cancelToken)
    {
        // --> Configuration
        MqttClientOptions clientOptions = Settings.GetMqttClientOptions();

        ManagedMqttClientOptions managedClientOptions = new ManagedMqttClientOptionsBuilder()
            .WithClientOptions(clientOptions)
            .Build();

        var mqttFactory = new MqttFactory();

        // --> Create client
        using IManagedMqttClient client = mqttFactory.CreateManagedMqttClient();

        // --> Start client
        await client.StartAsync(managedClientOptions);

        Console.WriteLine("Connect started");

        // --> Start: Publisher
        Task publisherTask = publisher(client, cancelToken);

        // --> Subscribe to topic
        await client.SubscribeAsync(Settings.Topic);

        client.ApplicationMessageReceivedAsync += client_MessageReceived;

        // --> Wait for the publisher until cancelToken closed
        await publisherTask;

        // --> Disconnect
        // Before stop the client, make sure no pending messages
        SpinWait.SpinUntil(() => client.PendingApplicationMessagesCount == 0, TimeSpan.FromSeconds(5));

        await client.StopAsync();
    }

    private static async Task client_MessageReceived(MqttApplicationMessageReceivedEventArgs arg)
    {
        string topic = arg.ApplicationMessage.Topic;
        //string payload = arg.ApplicationMessage.ConvertPayloadToString();

        Message? messageObject = JsonSerializer.Deserialize<Message>(arg.ApplicationMessage.PayloadSegment);

        await Console.Out.WriteLineAsync($"'{topic}' - Payload: '{messageObject}'");

        // await arg.AcknowledgeAsync(CancellationToken.None); // Not sure about it
    }

    private static async Task publisher(IManagedMqttClient client, CancellationToken cancelToken)
    {
        while (!cancelToken.IsCancellationRequested)
        {
            MqttApplicationMessage message = createMessage();

            await client.EnqueueAsync(message);

            // await client.EnqueueAsync(Settings.Topic, payload); // Simply send a Payload as string without the need of ApplicationMessage

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
