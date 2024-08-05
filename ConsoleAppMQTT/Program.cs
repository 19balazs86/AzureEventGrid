namespace ConsoleAppMQTT;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        Task exampleTask = MqttClientExample.Run(cancellationTokenSource.Token);
        // Task exampleTask = ManagedMqttClientExample.Run(cancellationTokenSource.Token);

        await Task.WhenAny(exampleTask, readKey());

        cancellationTokenSource.Cancel();

        Console.WriteLine("Program is closing");

        await exampleTask; // Wait for closing the connection

        Console.WriteLine("End of MQTT example");
    }

    private static Task readKey()
    {
        Console.WriteLine("Press any key to stop");
        Console.ReadKey(true);

        return Task.CompletedTask;
    }
}
