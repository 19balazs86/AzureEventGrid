using Azure;
using Microsoft.Extensions.Azure;
using NamespacesWorker.Miscellaneous;
using NamespacesWorker.Workers;

namespace NamespacesWorker;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        IServiceCollection services  = builder.Services;
        IConfiguration configuration = builder.Configuration;

        NamespaceSettings settings = configuration.GetConfigurationSection<NamespaceSettings>()!;

        // Add services to the container
        {
            services.AddHostedService<SenderWorker>();
            services.AddHostedService<ReceiverWorker>();
            services.AddHostedService<ReceiverSimpleWorker>();

            services.AddAzureClients(clients =>
            {
                clients.AddEventGridSenderClient(settings.EndpointUri, settings.Topic, new AzureKeyCredential(settings.AccessKey));

                clients.AddEventGridReceiverClient(settings.EndpointUri, settings.Topic, settings.Subscription, new AzureKeyCredential(settings.AccessKey));
            });
        }

        IHost host = builder.Build();

        host.Run();
    }
}