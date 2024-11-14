using MQTTnet.Client;
using System.Security.Cryptography.X509Certificates;

namespace ConsoleAppMQTT;

public static class Settings
{
    public const string ClientId = "client1-session1";

    public const string UserName = "client1-authn-ID";

    public const string Topic = "test-topics/topic1";

    public const string CertPemFilePath = @"c:\Users\Balazs\.step\client1-authn-ID.crt";

    public const string KeyPemFilePath = @"c:\Users\Balazs\.step\client1-authn-ID.key";

    public static string Hostname => Environment.GetEnvironmentVariable("MQTT__Hostname")
        ?? throw new NullReferenceException("Missing environment variable: MQTT__Hostname");

    public static MqttClientOptions GetMqttClientOptions()
    {
        MqttClientOptions clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(Hostname) // port: 8883 by default
            .WithClientId(ClientId)
            .WithCredentials(UserName)
            .WithTlsOptions(builder => builder.WithClientCertificates(getClientCertificates()))
            .WithWillQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce)
            .Build();

        return clientOptions;
    }

    private static X509Certificate2[] getClientCertificates()
    {
        var certificate = X509Certificate2.CreateFromPemFile(CertPemFilePath, KeyPemFilePath);

        byte[] bytes = certificate.Export(X509ContentType.Pkcs12);

        certificate = X509CertificateLoader.LoadPkcs12(bytes, password: null);

        return [certificate];
    }
}
