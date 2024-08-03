namespace NamespacesWorker.Miscellaneous;

public sealed class NamespaceSettings
{
    public required string EndpointUrl { get; init; }
    public required string AccessKey { get; init; }
    public required string Topic { get; init; }
    public required string Subscription { get; init; }

    public Uri EndpointUri => new Uri(EndpointUrl);

    public static NamespaceSettings CreateFrom(IConfiguration configuration)
    {
        return configuration.GetSection("EventGridNamespace").Get<NamespaceSettings>()!;
    }
}
