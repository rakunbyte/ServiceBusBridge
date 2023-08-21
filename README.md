# ServiceBusBridge

Microservice architecture for standing up some Kafka and AzureService Bus clients 
and processing their events.

### How it works

For each `KafkaConsumerServiceConfig` and `AzureServiceBusConsumerConfig` in your settings,
this will create a new `KafkaConsumerService` and `AzureServiceBusConsumerService`, 
which each hosts either a `KafkaClient` or `AzureServiceBusClient` respectively.

This will allow you to have as many consumers as you want, all depending on the config.

In the ConsumerConfigs you define a BackingServiceName - This points to a `BackingService` 
in the BackingServices Project, which handles deserialization of the Event and how to send the outgoing Event.

`PassthroughEvents` are events that are a direct transformation of the incoming Event to an outgoing Event, so there's
an abstract `PassthroughBackingService` to hide this logic.

### How to use it
* Add a new EventModel
* Add a new BackingService
* Register the BackingService in the DI
* Add a new ConsumerConfig that points to that BackingService
* Start!

### To Test
* Have a Kafka installed to localhost:29092 or update the `KafkaProducer` to use your Kafka.
* In the Testing solution folder there is a `KafkaProducer`.  Run that to produce messages
* Wait and run the ServiceBusBridge
