#Aitako.CQRS 
 
A friendly and easy to use [CQRS](http://abdullin.com/cqrs) framework with a fluid configuration interface which wires up some commonly used components common in a CQRS implementation using event sourcing. 

The framework uses [MassTransit](http://masstransit-project.com/) as a message bus wrapper, which means that you could use either [RabbitMQ](http://rabbitmq.com) or [MSMQ](http://en.wikipedia.org/wiki/Microsoft_Message_Queuing) as an event queue. 
Events are persisted through the excellent [NEventStore](http://neventstore.org) library which has support for a wide range of databases including [MongoDB](http://www.mongodb.org), [RavenDB](http://ravendb.net) and various flavours of SQL.

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

* integration with RabbitMQ through MassTransit
* integration with NEventStore using any of the supported persistence adapters. (Mongo, Raven, MS/MySQL)
* a very basic low-volume in-memory aggregate cache (for testing and small projects)
* a mongo aggregate cache which saves the latest version of each aggregate to disc using MongoDB
* dependency injection using StructureMap

###Future Plans
The framework is easy to extend and there are plans in the pipeline to support the following features:

*	Distributed command handler
*	Auto-generated SignalR endpoints for decorated aggregate events
*	Auto-generated ASP.NET MVC controller endpoints for calling domain commands
*	The ability to run an in-memory environment for prototyping purposes based purely on the domain specification file. (long term goal)

###The framework consists of the following core components:
#####Core
The core provides the most basic functionality as well as base classes and interfaces for defining aggregates, services, commands and events. Let's look at a few examples:
######Commands
######Events
######Aggregates
######Services
#####DSL
_coming soon_
#####Interceptor
_coming soon_
#####Aggregate Update Publisher Extension
_coming soon_
#####MongoDB Storage Extension
_coming soon_
#####NEventStore Extension
_coming soon_
#####Basic In Memory Cache Extension
_coming soon_
