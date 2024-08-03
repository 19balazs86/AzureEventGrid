namespace NamespacesWorker;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        IServiceCollection services  = builder.Services;
        IConfiguration configuration = builder.Configuration;

        // Add services to the container
        {
            services.AddHostedService<Worker>();
        }

        IHost host = builder.Build();

        host.Run();
    }
}