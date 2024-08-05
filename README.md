# Playing with Azure Event Grid

This repository contains 2 projects to explore Azure Event Grid -> Namespaces

- Feature: **Namespaces Topic** via an HTTP sender and receiver
- Feature: **MQTT** pub-sub message broker

## Prerequisites

- To create any kind of Event Grid services, first you need to navigate to your Azure Subscription / Settings / Resource providers to [register the Microsoft.EventGrid provider](https://learn.microsoft.com/en-us/azure/event-grid/custom-event-quickstart-portal#register-the-event-grid-resource-provider)
- Navigate to "Event Grid Namespaces" on Azure Portal using the 'search resources' text-box in the middle
- Create a namespace with default values

## Projects in the solution

#### `NamespacesWorker`

- Worker service with a sender and receiver
- Configure the appsettings.json

```json
"EventGridNamespace": {
    "EndpointUrl": "Overview page: 'HTTP hostname' and append the https:// prefix",
    "AccessKey": "Settings / Access keys, use the Key 1 | You can use the created Topic's access key",
    "Topic": "Name of your created Topic",
    "Subscription": "Name of your created Subscription for the Topic"
}
```

#### `ConsoleAppMQTT`

- Console application publish and subscribe to a topic
- Prerequisites
  - [Enable MQTT broker and create Clients](https://learn.microsoft.com/en-us/azure/event-grid/mqtt-publish-and-subscribe-portal)
  - Configure the Settings.cs file based on the previous step
- [MqttClientExample.cs](ConsoleAppMQTT/MqttClientExample.cs): Simply using the MqttClient from the base library
- [ManagedMqttClientExample.cs](ConsoleAppMQTT/ManagedMqttClientExample.cs): Using the ManagedMqttClient, a wrapper for the base MqttClient with additional features. It does not publish messages directly but uses an internal queue and so on...

## Resources

#### Event Grid - General

- [Documentation](https://learn.microsoft.com/en-us/azure/event-grid) 📚*MS-Learn*

#### Namespaces - Topic

- Quick start: [Send and receive messages via Namespace Topic](https://learn.microsoft.com/en-us/azure/event-grid/event-grid-dotnet-get-started-pull-delivery)
- [Azure.Messaging.EventGrid.Namespaces - Namespace](https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.eventgrid.namespaces?view=azure-dotnet)
- [Source code + Samples](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.EventGrid.Namespaces_1.0.0/sdk/eventgrid/Azure.Messaging.EventGrid.Namespaces/samples) 👤*Azure SDK*

#### Namespaces - MQTT

- Quick start: [Publish and subscribe to MQTT messages](https://learn.microsoft.com/en-us/azure/event-grid/mqtt-publish-and-subscribe-portal) 📚*MS-learn*
  - [Client authentication with the uploaded certificate](https://learn.microsoft.com/en-us/azure/event-grid/mqtt-certificate-chain-client-authentication) 📚*MS-learn*
- [Download MQTT Client](https://mqttx.app)
- Samples
  - [MQTTnet + samples](https://github.com/dotnet/MQTTnet) 👤*DotNET*
  - [MqttApplicationSamples](https://github.com/Azure-Samples/MqttApplicationSamples) 👤*Azure-Samples*
