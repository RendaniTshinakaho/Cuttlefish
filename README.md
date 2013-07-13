#Aitako.CQRS 
 
A friendly and easy to use [CQRS](http://abdullin.com/cqrs) framework with a fluid configuration interface which wires up some commonly used components common in a CQRS implementation using event sourcing. 

The framework uses [MassTransit](http://masstransit-project.com/) as a message bus wrapper, which means that you could use either [RabbitMQ](http://rabbitmq.com) or [MSMQ](http://en.wikipedia.org/wiki/Microsoft_Message_Queuing) as an event queue. 
Events are persisted through the excellent [NEventStore](http://neventstore.org) library which has support for a wide range of databases including [MongoDB](http://www.mongodb.org), [RavenDB](http://ravendb.net) and various flavours of SQL.

A basic and functional example of a domain built with the framework is included under the Cuttlefish.ExampleApp.Domain folder. It illustrates the use of commands, aggregates, services and events in the context of the Cuttlefish framework.

####NOT READY FOR USE IN PRODUCTION YET. A lot of documentation still needs to be written and some refactoring and tweaking still needs to take place before I'd start using this on smaller projects.

#####Quick Setup
Getting the framework up and running is super easy. Assuming that we are using RabbitMQ with a MongoDB event store backing, we would set up the framework as follows:

```
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
The core provides the most basic functionality as well as base classes and interfaces for defining aggregates, services, commands and events. 

The 4 things you'll need to work with to start setting up your domain are commands, events, services and aggregates.

######Commands
We'll start with commands. They tell our domain what to do. They can be defined by implementing the ICommand interface as below. Commands are handled by either aggregates or services, but we'll get to how that works in a bit.

```
public class StartStockingProduct : ICommand
{
    private readonly int _version;

    private StartStockingProduct()
    {
        _version = 1;
    }

    public StartStockingProduct(Guid aggregateidentity, String itemcode, String name, String description,
                                String barcode) : this()
    {
        AggregateIdentity = aggregateidentity;
        ItemCode = itemcode;
        Name = name;
        Description = description;
        Barcode = barcode;
    }

    public String ItemCode { get; private set; }
    public String Name { get; private set; }
    public String Description { get; private set; }
    public String Barcode { get; private set; }

    public int Version
    {
        get { return _version; }
    }

    public Guid AggregateIdentity { get; private set; }
}
```

In the example above, we are creating a command which tells the warehouse service to start stocking a certain product. The command conveys information that would be required by the domain to perform the action the command triggers.

To execute commands against domain services or aggregates, we make use of the CommandRouter. The router is clever enough to know which service or aggregate should react to the command. Only a single handler may be registered to handle commands. The code below illustrates how we would execute a command through the router.

```
_productName = "Widget X";
_description = "blah blah blah";
_itemcode = "X0001";
_barcode = "123456";

var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, _productName, _description, _barcode);

CommandRouter.ExecuteCommand(newProductCommand);
```

In the example above, the command router knows which handler the command should be routed to. The service or aggregate would then respond by persisting an event of what has been executed to the eventstore. More on that later...

######Services & Aggregates

What do these aggregates and services look like? You might ask. Well, here is an example of a service which models the way a warehouse might work in an over-simplified business scenario.

######Events

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
