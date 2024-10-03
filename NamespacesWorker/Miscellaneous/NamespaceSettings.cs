namespace NamespacesWorker.Miscellaneous;

public sealed class NamespaceSettings : IConfigOptions
{
    public static string SectionName { get; } = "EventGridNamespace";

    public required string EndpointUrl { get; init; }
    public required string AccessKey { get; init; }
    public required string Topic { get; init; }
    public required string Subscription { get; init; }

    public Uri EndpointUri => new Uri(EndpointUrl);
}
