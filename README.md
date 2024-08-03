# Playing with Azure Event Grid

This repository contains a project to explore Azure Event Grid -> Namespaces -> Topic via an HTTP sender and receiver library.

## Prerequisites

- To create any kind of Event Grid services, first you need to navigate to your Azure Subscription / Settings / Resource providers to [register the Microsoft.EventGrid provider](https://learn.microsoft.com/en-us/azure/event-grid/custom-event-quickstart-portal#register-the-event-grid-resource-provider)
- On Azure Portal navigate to "Event Grid Namespaces" using the search resources text-box in the middle
- Create a namespace with default values
- Create a Topic and a Subscription

Configure the appsettings.json

```json
"EventGridNamespace": {
    "EndpointUrl": "Overview page: 'HTTP hostname' and append the https:// prefix",
    "AccessKey": "Settings / Access keys, use the Key 1 | You can use the created Topic's access key",
    "Topic": "Name of your created Topic",
    "Subscription": "Name of your created Subscription for the Topic"
}
```

## Resources

- [Documentation](https://learn.microsoft.com/en-us/azure/event-grid) 📚*MS-Learn*
  - [Quick start: Send and receive messages from an Event Grid namespace topic](https://learn.microsoft.com/en-us/azure/event-grid/event-grid-dotnet-get-started-pull-delivery)
  - [Azure.Messaging.EventGrid.Namespaces - Namespace](https://learn.microsoft.com/en-us/dotnet/api/azure.messaging.eventgrid.namespaces?view=azure-dotnet)
- [Source code + Samples](https://github.com/Azure/azure-sdk-for-net/tree/Azure.Messaging.EventGrid.Namespaces_1.0.0/sdk/eventgrid/Azure.Messaging.EventGrid.Namespaces/samples) 👤*Azure SDK*