using Azure;
using Microsoft.Extensions.Azure;
using NamespacesWorker.Workers;

namespace NamespacesWorker;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        IServiceCollection services  = builder.Services;
        IConfiguration configuration = builder.Configuration;

        NamespaceSettings settings = NamespaceSettings.CreateFrom(configuration);

        // Add services to the container
        {
            services.AddHostedService<Worker>();

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