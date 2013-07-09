#Aitako.CQRS 
 
A friendly and easy to use CQRS framework with a fluid configuration interface which attempts to wire up some commonly used components to make up a nifty little CQRS architecture using event sourcing. 

The framework currently uses MassTransit as a message bus wrapper, which means that you could use either RabbitMQ or MSMQ for event queues. Events are persisted through an injected instance of the excellent http://neventstore.org/ library which has support for a wide range of databases.

####NOT READY FOR USE IN PRODUCTION YET. I'm looking at having the code and documentation stable enough (with the current feature set) for small projects by August 2013. 

#####Quick Setup
Getting the framework up and running is super easy. Assuming that we are using RabbitMQ with a MongoDB event store backing, we would set up the framework as follows:

```C#
// Starts up the configuration interface
Core.Configure()

	// Specify which root namespace reflection should use for finding aggregates, 
	// events and commands.
    .WithDomainNamespaceRoot("MyCompany.MyApp.Domain")

    // For testing and extension purposes only at present, specifies which in-memory cache to 
	// use for storing the latest aggregate state
	.UseInMemoryCache()

    // Configures NEventStore, more info available on https://github.com/NEventStore/NEventStore
	.UseNEventStore(Wireup.Init()
		.UsingMongoPersistence("eventstore", new DocumentObjectSerializer())
			.Build())

    // Configures the aggregate update publisher, which will push entire aggregates to the 
	// message queue for consumption by Interceptors later.
	.UseMassTransitAggregateUpdatePublisher(ServiceBusFactory.New(sbc =>
    {
        sbc.ReceiveFrom("rabbitmq://localhost/aggregate_updates");
    }))

	// Wires everything up for us.
    .Done();
```

This will wire up all the components required by the framework.  Out of the box, you get:
+ integration with RabbitMQ through MassTransit
+ integration with NEventStore using any of the supported persistence adapters. (Mongo, Raven, MS/MySQL)
+ a very basic low-volume in-memory aggregate cache (for testing and small projects)
+ a mongo aggregate cache which saves the latest version of each aggregate to disc using MongoDB
+ dependency injection using StructureMap

###The framework consists of the following core components:
#####Core
#####DSL
#####Interceptor
#####Aggregate Update Publisher Extension
#####MongoDB Storage Extension
#####NEventStore Extension
#####Basic In Memory Cache Extension
