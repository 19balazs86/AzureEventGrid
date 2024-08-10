namespace ConsoleAppMQTT;

public static class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Press Ctrl+C to stop Mqtt client");

        using var cancellationTokenSource = cancelKeyPress();

        await MqttClientExample.Run(cancellationTokenSource.Token);

        // await ManagedMqttClientExample.Run(cancellationTokenSource.Token);
    }

    private static CancellationTokenSource cancelKeyPress()
    {
        var cancellationTokenSource = new CancellationTokenSource();

        Console.CancelKeyPress += (object? sender, ConsoleCancelEventArgs eventArgs) =>
        {
            Console.WriteLine("Program is closing");

            eventArgs.Cancel = true; // The current process continues, not killing the app. Mqtt client can disconnect properly

            cancellationTokenSource.Cancel();
        };

        return cancellationTokenSource;
    }
}
