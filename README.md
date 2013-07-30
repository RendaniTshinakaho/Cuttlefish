 
A friendly and easy to use framework based on [CQRS](http://abdullin.com/cqrs) principles with a fluid configuration interface which wires up some commonly used components common in a CQRS implementation using event sourcing. It includes a simple and fluid tool for defining aggregates, events, services and commands which generates the skeleton code required for all of these items, saving time and achieving consistency.

The framework uses [MassTransit](http://masstransit-project.com/) as a message bus wrapper, which means that you could use either [RabbitMQ](http://rabbitmq.com) or [MSMQ](http://en.wikipedia.org/wiki/Microsoft_Message_Queuing) as an event queue. 
Events are persisted through the excellent [NEventStore](http://neventstore.org) library which has support for a wide range of databases including [MongoDB](http://www.mongodb.org), [RavenDB](http://ravendb.net) and various flavours of SQL.

A basic and functional example of a domain built with the framework is included under the Cuttlefish.ExampleApp.Domain folder. It illustrates the use of commands, aggregates, services and events in the context of the Cuttlefish framework.

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
The core provides the most basic functionality as well as base classes and interfaces for defining aggregates, services, commands and events. 

The 4 things you'll need to work with to start setting up your domain are commands, events, services and aggregates.

######Commands
We'll start with commands. They tell our domain what to do. They can be defined by implementing the ICommand interface as below. Commands are handled by either aggregates or services, but we'll get to how that works in a bit.

```C#
public class StartStockingProduct : ICommand
{
    private readonly int _version;

    private StartStockingProduct()
    {
        _version = 1;
    }

    public StartStockingProduct(Guid aggregateidentity, String itemcode, String name, 
                                String description, String barcode) : this()
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

```C#
_productName = "Widget X";
_description = "blah blah blah";
_itemcode = "X0001";
_barcode = "123456";

var newProductCommand = new StartStockingProduct(Guid.NewGuid(), _itemcode, 
    _productName, _description, _barcode);

CommandRouter.ExecuteCommand(newProductCommand);
```

In the example above, the command router knows which handler the command should be routed to. The service or aggregate would then respond by persisting an event of what has been executed to the eventstore. More on that later...

######Services

What do these aggregates and services look like? You might ask. Well, here is an example of a service which models the way a warehouse might work in an over-simplified business scenario.

```C#
public class WarehouseService : IService
{
    public void On(StartStockingProduct cmd)
    {
        if (!BarcodeLengthIsCorrect(cmd.Barcode))
        {
            throw new InvalidBarcodeException(cmd.Barcode);
        }

        if (!NameIsValid(cmd.Name))
        {
            throw new ProductStockingException(cmd);
        }

        EventRouter.FireEventOnAggregate<ProductAggregate>(
            new NewProductAddedToWarehouse(cmd.AggregateIdentity,
                                                  cmd.ItemCode, cmd.Name,
                                                  cmd.Description,
                                                  cmd.Barcode));
    }

    public void On(Rename cmd)
    {
        if (!NameIsValid(cmd.Name))
        {
            throw new ProductStockingException(cmd);
        }

        EventRouter.FireEventOnAggregate<ProductAggregate>(new Renamed(cmd.AggregateIdentity, 
            cmd.Name));
    }

    public void On(AcceptShipmentOfProduct cmd)
    {
        if (!IsValidQuantity(cmd.Quantity))
        {
            throw new InvalidQuantityException();
        }

        EventRouter.FireEventOnAggregate<ProductAggregate>(new StockReceived(cmd.AggregateIdentity, 
            cmd.Quantity));
    }

    public void On(BookOutStockAgainstOrder cmd)
    {
        if (!IsValidQuantity(cmd.Quantity))
        {
            throw new InvalidQuantityException();
        }

        if (!ItemIsInStockForQuantityRequired(cmd.AggregateIdentity, cmd.Quantity))
        {
            throw new OutOfStockException();
        }

        EventRouter.FireEventOnAggregate<ProductAggregate>(new StockBookedOut(cmd.AggregateIdentity, 
            cmd.Quantity));
    }

    public void On(SuspendSaleOfProduct cmd)
    {
        EventRouter.FireEventOnAggregate<ProductAggregate>(new Suspended(cmd.AggregateIdentity));
    }

    public void On(DiscontinueProduct cmd)
    {
        EventRouter.FireEventOnAggregate<ProductAggregate>(new Discontinued(cmd.AggregateIdentity));
    }

    #region Validation Rules

    private static bool ItemIsInStockForQuantityRequired(Guid aggregateIdentity, int quantityRequired)
    {
        var product = AggregateBuilder.Get<ProductAggregate>(aggregateIdentity);
        return product.QuantityOnHand >= quantityRequired;
    }

    private static bool BarcodeLengthIsCorrect(string barcode)
    {
        const int barcodeLength = 6;
        return barcode.Length == barcodeLength;
    }

    private static bool NameIsValid(string name)
    {
        return !string.IsNullOrEmpty(name);
    }

    private static bool IsValidQuantity(int quantity)
    {
        return quantity > 0;
    }
    #endregion
}
```

From the code above, we can see that services implement the _IService_ interface, which is used to decorate the resulting class as being a service.

We also see a bunch of _On_ methods; each accepting a single command object as a parameter. These are the command handlers. Aggregates are also able to have command handlers, in cases where it makes sense to model the domain in such a manner. The role of a command handler is to establish whether a command is valid and the state of the aggregate in question permits the action which the command is trying to execute. If all goes well and the command is able to execute without failing business rule validation, an event is created to signify that an action has taken place.

The event router is used to publish events from Services. This is done by simply calling the event router and providing an event with the required parameters and type.

```C#
EventRouter.FireEventOnAggregate<ProductAggregate>(new Discontinued(cmd.AggregateIdentity));
```

This will cause an event to be persisted to the event store; the event to be published to the messagebus (if specified in the core setup); and if enabled, it will also cause the latest version of the aggregate which responds to the event (if any) to be published to the specified cache.

The last thing we see in our service, is that there are a few methods that wrap business logic or validation logic. It is a good idea to put these into separate methods, as business logic is often re-used within an application domain.

######Events 

Events are very similar to commands in that they are merely carriers of information. In the case of commands, we wish to _carry intent_. In the case of events, we wish to carry _'what has been done'_. 

```C#
public class StockReceived : IEvent
{
    private readonly int _version;

    private StockReceived()
    {
        _version = 1;
    }

    public StockReceived(Guid aggregateidentity, Int32 quantity) : this()
    {
        AggregateIdentity = aggregateidentity;
        Quantity = quantity;
    }

    public Int32 Quantity { get; private set; }

    public int Version
    {
        get { return _version; }
    }

    public Guid AggregateIdentity { get; private set; }
}
```
The event above is raised when the _AcceptShipmentOfProduct_ command is handled in our service. We could have easily put the command handler on the _ProductAggregate_ if we wanted to, but in this case, it made more semantic sense to let the warehouse take care of accepting shipments.

The event only contains information that is required to process the fact that stock has been received. In the next section we will take a look at the way events are handled by aggregates.

######Aggregates

Aggregates are the meat of our domain. They are generally models of real-world items or documents and should be rich and extensive. In the example below, we are modeling a product from a warehouse perspective. The warehouse doesn't care about pricing information, as its main area of concern is stock levels and maintaining a catalogue.

```C#
public class ProductAggregate : AggregateBase
{
    public ProductAggregate() : base(new List<IEvent>())
    {
    }

    public ProductAggregate(IEnumerable<IEvent> events) : base(events)
    {
    }

    public DateTime LastChanged { get; private set; }
    public String ItemCode { get; private set; }
    public String Name { get; private set; }
    public String Description { get; private set; }
    public String Barcode { get; private set; }
    public Boolean Suspended { get; private set; }
    public Boolean Discontinued { get; private set; }
    public Int32 QuantityOnHand { get; private set; }

    public void When(NewProductAddedToWarehouse evt)
    {
        AggregateIdentity = evt.AggregateIdentity;
        Barcode = evt.Barcode;
        Name = evt.Name;
        Description = evt.Description;
        Discontinued = false;
        Suspended = false;
        ItemCode = evt.ItemCode;
        LastChanged = evt.Timestamp;
        QuantityOnHand = 0;
    }

    public void When(Renamed evt)
    {
        Name = evt.Name;
    }

    public void When(Discontinued evt)
    {
        Discontinued = true;
    }

    public void When(Suspended evt)
    {
        Suspended = true;
    }

    public void When(StockReceived evt)
    {
        QuantityOnHand += evt.Quantity;
    }

    public void When(StockBookedOut evt)
    {
        QuantityOnHand -= evt.Quantity;
    }
}
```

We see that the _ProductAggregate_ inherits from _AggregateBase_ and has a constructor which accepts a set of events. These events are effectively a history of what has happened to the product throughout its life.

There are various _When_ methods, which are capable of accepting individual events as parameters. Each _When_ method acts upon the event by responding in a way that mimics what would happen in the business. For instance, we see that the _Renamed_ event changes the name on the aggregate and the _StockReceived_ and _StockBookedOut_ events control the level of stock for the product. 

The reason we pass a set of events into the constructor is that the base class executes the associated When method for each event in that set to rehydrate the aggregate based on actual history. This allows us to change how the domain responds to certain events historically. The code below illustrates what the base constructor does.

```C#
foreach (IEvent @event in events)
{
    InvokeEvent(this, @event);
}
```

The _InvokeEvent_ method simply calls the required When method on the aggregate in sequence. By the time events are passed to the constructore, they are already sorted by timestamp.

Events have version numbers attached to them, so we have the ability to change our domain logic based on different sets of events. This makes dealing with change a bit less painful. For extensive changes in the domain model, it is suggested that a migration approach be followed and that events are rewritten.

That's cool and stuff, but how the hell do I get hold of an aggregate once I've created it?
```C#
var product = AggregateBuilder.Get<ProductAggregate>(_productId);
```
Aggregates have GUIDs for unique identifiers. The AggregateBuilder.Get method builds an aggregate from its events that are fetched from the event store based on its unique key.

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
##### License
THE WORK (AS DEFINED BELOW) IS PROVIDED UNDER THE TERMS OF THIS CREATIVE COMMONS PUBLIC LICENSE ("CCPL" OR "LICENSE"). THE WORK IS PROTECTED BY COPYRIGHT AND/OR OTHER APPLICABLE LAW. ANY USE OF THE WORK OTHER THAN AS AUTHORIZED UNDER THIS LICENSE OR COPYRIGHT LAW IS PROHIBITED.
BY EXERCISING ANY RIGHTS TO THE WORK PROVIDED HERE, YOU ACCEPT AND AGREE TO BE BOUND BY THE TERMS OF THIS LICENSE. TO THE EXTENT THIS LICENSE MAY BE CONSIDERED TO BE A CONTRACT, THE LICENSOR GRANTS YOU THE RIGHTS CONTAINED HERE IN CONSIDERATION OF YOUR ACCEPTANCE OF SUCH TERMS AND CONDITIONS.
1. Definitions
"Adaptation" means a work based upon the Work, or upon the Work and other pre-existing works, such as a translation, adaptation, derivative work, arrangement of music or other alterations of a literary or artistic work, or phonogram or performance and includes cinematographic adaptations or any other form in which the Work may be recast, transformed, or adapted including in any form recognizably derived from the original, except that a work that constitutes a Collection will not be considered an Adaptation for the purpose of this License. For the avoidance of doubt, where the Work is a musical work, performance or phonogram, the synchronization of the Work in timed-relation with a moving image ("synching") will be considered an Adaptation for the purpose of this License. "Collection" means a collection of literary or artistic works, such as encyclopedias and anthologies, or performances, phonograms or broadcasts, or other works or subject matter other than works listed in Section 1(f) below, which, by reason of the selection and arrangement of their contents, constitute intellectual creations, in which the Work is included in its entirety in unmodified form along with one or more other contributions, each constituting separate and independent works in themselves, which together are assembled into a collective whole. A work that constitutes a Collection will not be considered an Adaptation (as defined below) for the purposes of this License. "Creative Commons Compatible License" means a license that is listed at http://creativecommons.org/compatiblelicenses that has been approved by Creative Commons as being essentially equivalent to this License, including, at a minimum, because that license: (i) contains terms that have the same purpose, meaning and effect as the License Elements of this License; and, (ii) explicitly permits the relicensing of adaptations of works made available under that license under this License or a Creative Commons jurisdiction license with the same License Elements as this License. "Distribute" means to make available to the public the original and copies of the Work or Adaptation, as appropriate, through sale or other transfer of ownership. "License Elements" means the following high-level license attributes as selected by Licensor and indicated in the title of this License: Attribution, ShareAlike. "Licensor" means the individual, individuals, entity or entities that offer(s) the Work under the terms of this License. "Original Author" means, in the case of a literary or artistic work, the individual, individuals, entity or entities who created the Work or if no individual or entity can be identified, the publisher; and in addition (i) in the case of a performance the actors, singers, musicians, dancers, and other persons who act, sing, deliver, declaim, play in, interpret or otherwise perform literary or artistic works or expressions of folklore; (ii) in the case of a phonogram the producer being the person or legal entity who first fixes the sounds of a performance or other sounds; and, (iii) in the case of broadcasts, the organization that transmits the broadcast. "Work" means the literary and/or artistic work offered under the terms of this License including without limitation any production in the literary, scientific and artistic domain, whatever may be the mode or form of its expression including digital form, such as a book, pamphlet and other writing; a lecture, address, sermon or other work of the same nature; a dramatic or dramatico-musical work; a choreographic work or entertainment in dumb show; a musical composition with or without words; a cinematographic work to which are assimilated works expressed by a process analogous to cinematography; a work of drawing, painting, architecture, sculpture, engraving or lithography; a photographic work to which are assimilated works expressed by a process analogous to photography; a work of applied art; an illustration, map, plan, sketch or three-dimensional work relative to geography, topography, architecture or science; a performance; a broadcast; a phonogram; a compilation of data to the extent it is protected as a copyrightable work; or a work performed by a variety or circus performer to the extent it is not otherwise considered a literary or artistic work. "You" means an individual or entity exercising rights under this License who has not previously violated the terms of this License with respect to the Work, or who has received express permission from the Licensor to exercise rights under this License despite a previous violation. "Publicly Perform" means to perform public recitations of the Work and to communicate to the public those public recitations, by any means or process, including by wire or wireless means or public digital performances; to make available to the public Works in such a way that members of the public may access these Works from a place and at a place individually chosen by them; to perform the Work to the public by any means or process and the communication to the public of the performances of the Work, including by public digital performance; to broadcast and rebroadcast the Work by any means including signs, sounds or images. "Reproduce" means to make copies of the Work by any means including without limitation by sound or visual recordings and the right of fixation and reproducing fixations of the Work, including storage of a protected performance or phonogram in digital form or other electronic medium. 2. Fair Dealing Rights. Nothing in this License is intended to reduce, limit, or restrict any uses free from copyright or rights arising from limitations or exceptions that are provided for in connection with the copyright protection under copyright law or other applicable laws.
3. License Grant. Subject to the terms and conditions of this License, Licensor hereby grants You a worldwide, royalty-free, non-exclusive, perpetual (for the duration of the applicable copyright) license to exercise the rights in the Work as stated below:
to Reproduce the Work, to incorporate the Work into one or more Collections, and to Reproduce the Work as incorporated in the Collections; to create and Reproduce Adaptations provided that any such Adaptation, including any translation in any medium, takes reasonable steps to clearly label, demarcate or otherwise identify that changes were made to the original Work. For example, a translation could be marked "The original work was translated from English to Spanish," or a modification could indicate "The original work has been modified."; to Distribute and Publicly Perform the Work including as incorporated in Collections; and, to Distribute and Publicly Perform Adaptations. For the avoidance of doubt:
Non-waivable Compulsory License Schemes. In those jurisdictions in which the right to collect royalties through any statutory or compulsory licensing scheme cannot be waived, the Licensor reserves the exclusive right to collect such royalties for any exercise by You of the rights granted under this License; Waivable Compulsory License Schemes. In those jurisdictions in which the right to collect royalties through any statutory or compulsory licensing scheme can be waived, the Licensor waives the exclusive right to collect such royalties for any exercise by You of the rights granted under this License; and, Voluntary License Schemes. The Licensor waives the right to collect royalties, whether individually or, in the event that the Licensor is a member of a collecting society that administers voluntary licensing schemes, via that society, from any exercise by You of the rights granted under this License. The above rights may be exercised in all media and formats whether now known or hereafter devised. The above rights include the right to make such modifications as are technically necessary to exercise the rights in other media and formats. Subject to Section 8(f), all rights not expressly granted by Licensor are hereby reserved.
4. Restrictions. The license granted in Section 3 above is expressly made subject to and limited by the following restrictions:
You may Distribute or Publicly Perform the Work only under the terms of this License. You must include a copy of, or the Uniform Resource Identifier (URI) for, this License with every copy of the Work You Distribute or Publicly Perform. You may not offer or impose any terms on the Work that restrict the terms of this License or the ability of the recipient of the Work to exercise the rights granted to that recipient under the terms of the License. You may not sublicense the Work. You must keep intact all notices that refer to this License and to the disclaimer of warranties with every copy of the Work You Distribute or Publicly Perform. When You Distribute or Publicly Perform the Work, You may not impose any effective technological measures on the Work that restrict the ability of a recipient of the Work from You to exercise the rights granted to that recipient under the terms of the License. This Section 4(a) applies to the Work as incorporated in a Collection, but this does not require the Collection apart from the Work itself to be made subject to the terms of this License. If You create a Collection, upon notice from any Licensor You must, to the extent practicable, remove from the Collection any credit as required by Section 4(c), as requested. If You create an Adaptation, upon notice from any Licensor You must, to the extent practicable, remove from the Adaptation any credit as required by Section 4(c), as requested. You may Distribute or Publicly Perform an Adaptation only under the terms of: (i) this License; (ii) a later version of this License with the same License Elements as this License; (iii) a Creative Commons jurisdiction license (either this or a later license version) that contains the same License Elements as this License (e.g., Attribution-ShareAlike 3.0 US)); (iv) a Creative Commons Compatible License. If you license the Adaptation under one of the licenses mentioned in (iv), you must comply with the terms of that license. If you license the Adaptation under the terms of any of the licenses mentioned in (i), (ii) or (iii) (the "Applicable License"), you must comply with the terms of the Applicable License generally and the following provisions: (I) You must include a copy of, or the URI for, the Applicable License with every copy of each Adaptation You Distribute or Publicly Perform; (II) You may not offer or impose any terms on the Adaptation that restrict the terms of the Applicable License or the ability of the recipient of the Adaptation to exercise the rights granted to that recipient under the terms of the Applicable License; (III) You must keep intact all notices that refer to the Applicable License and to the disclaimer of warranties with every copy of the Work as included in the Adaptation You Distribute or Publicly Perform; (IV) when You Distribute or Publicly Perform the Adaptation, You may not impose any effective technological measures on the Adaptation that restrict the ability of a recipient of the Adaptation from You to exercise the rights granted to that recipient under the terms of the Applicable License. This Section 4(b) applies to the Adaptation as incorporated in a Collection, but this does not require the Collection apart from the Adaptation itself to be made subject to the terms of the Applicable License. If You Distribute, or Publicly Perform the Work or any Adaptations or Collections, You must, unless a request has been made pursuant to Section 4(a), keep intact all copyright notices for the Work and provide, reasonable to the medium or means You are utilizing: (i) the name of the Original Author (or pseudonym, if applicable) if supplied, and/or if the Original Author and/or Licensor designate another party or parties (e.g., a sponsor institute, publishing entity, journal) for attribution ("Attribution Parties") in Licensor's copyright notice, terms of service or by other reasonable means, the name of such party or parties; (ii) the title of the Work if supplied; (iii) to the extent reasonably practicable, the URI, if any, that Licensor specifies to be associated with the Work, unless such URI does not refer to the copyright notice or licensing information for the Work; and (iv) , consistent with Ssection 3(b), in the case of an Adaptation, a credit identifying the use of the Work in the Adaptation (e.g., "French translation of the Work by Original Author," or "Screenplay based on original Work by Original Author"). The credit required by this Section 4(c) may be implemented in any reasonable manner; provided, however, that in the case of a Adaptation or Collection, at a minimum such credit will appear, if a credit for all contributing authors of the Adaptation or Collection appears, then as part of these credits and in a manner at least as prominent as the credits for the other contributing authors. For the avoidance of doubt, You may only use the credit required by this Section for the purpose of attribution in the manner set out above and, by exercising Your rights under this License, You may not implicitly or explicitly assert or imply any connection with, sponsorship or endorsement by the Original Author, Licensor and/or Attribution Parties, as appropriate, of You or Your use of the Work, without the separate, express prior written permission of the Original Author, Licensor and/or Attribution Parties. Except as otherwise agreed in writing by the Licensor or as may be otherwise permitted by applicable law, if You Reproduce, Distribute or Publicly Perform the Work either by itself or as part of any Adaptations or Collections, You must not distort, mutilate, modify or take other derogatory action in relation to the Work which would be prejudicial to the Original Author's honor or reputation. Licensor agrees that in those jurisdictions (e.g. Japan), in which any exercise of the right granted in Section 3(b) of this License (the right to make Adaptations) would be deemed to be a distortion, mutilation, modification or other derogatory action prejudicial to the Original Author's honor and reputation, the Licensor will waive or not assert, as appropriate, this Section, to the fullest extent permitted by the applicable national law, to enable You to reasonably exercise Your right under Section 3(b) of this License (right to make Adaptations) but not otherwise. 5. Representations, Warranties and Disclaimer
UNLESS OTHERWISE MUTUALLY AGREED TO BY THE PARTIES IN WRITING, LICENSOR OFFERS THE WORK AS-IS AND MAKES NO REPRESENTATIONS OR WARRANTIES OF ANY KIND CONCERNING THE WORK, EXPRESS, IMPLIED, STATUTORY OR OTHERWISE, INCLUDING, WITHOUT LIMITATION, WARRANTIES OF TITLE, MERCHANTIBILITY, FITNESS FOR A PARTICULAR PURPOSE, NONINFRINGEMENT, OR THE ABSENCE OF LATENT OR OTHER DEFECTS, ACCURACY, OR THE PRESENCE OF ABSENCE OF ERRORS, WHETHER OR NOT DISCOVERABLE. SOME JURISDICTIONS DO NOT ALLOW THE EXCLUSION OF IMPLIED WARRANTIES, SO SUCH EXCLUSION MAY NOT APPLY TO YOU.
6. Limitation on Liability. EXCEPT TO THE EXTENT REQUIRED BY APPLICABLE LAW, IN NO EVENT WILL LICENSOR BE LIABLE TO YOU ON ANY LEGAL THEORY FOR ANY SPECIAL, INCIDENTAL, CONSEQUENTIAL, PUNITIVE OR EXEMPLARY DAMAGES ARISING OUT OF THIS LICENSE OR THE USE OF THE WORK, EVEN IF LICENSOR HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGES.
7. Termination
This License and the rights granted hereunder will terminate automatically upon any breach by You of the terms of this License. Individuals or entities who have received Adaptations or Collections from You under this License, however, will not have their licenses terminated provided such individuals or entities remain in full compliance with those licenses. Sections 1, 2, 5, 6, 7, and 8 will survive any termination of this License. Subject to the above terms and conditions, the license granted here is perpetual (for the duration of the applicable copyright in the Work). Notwithstanding the above, Licensor reserves the right to release the Work under different license terms or to stop distributing the Work at any time; provided, however that any such election will not serve to withdraw this License (or any other license that has been, or is required to be, granted under the terms of this License), and this License will continue in full force and effect unless terminated as stated above. 8. Miscellaneous
Each time You Distribute or Publicly Perform the Work or a Collection, the Licensor offers to the recipient a license to the Work on the same terms and conditions as the license granted to You under this License. Each time You Distribute or Publicly Perform an Adaptation, Licensor offers to the recipient a license to the original Work on the same terms and conditions as the license granted to You under this License. If any provision of this License is invalid or unenforceable under applicable law, it shall not affect the validity or enforceability of the remainder of the terms of this License, and without further action by the parties to this agreement, such provision shall be reformed to the minimum extent necessary to make such provision valid and enforceable. No term or provision of this License shall be deemed waived and no breach consented to unless such waiver or consent shall be in writing and signed by the party to be charged with such waiver or consent. This License constitutes the entire agreement between the parties with respect to the Work licensed here. There are no understandings, agreements or representations with respect to the Work not specified here. Licensor shall not be bound by any additional provisions that may appear in any communication from You. This License may not be modified without the mutual written agreement of the Licensor and You. The rights granted under, and the subject matter referenced, in this License were drafted utilizing the terminology of the Berne Convention for the Protection of Literary and Artistic Works (as amended on September 28, 1979), the Rome Convention of 1961, the WIPO Copyright Treaty of 1996, the WIPO Performances and Phonograms Treaty of 1996 and the Universal Copyright Convention (as revised on July 24, 1971). These rights and subject matter take effect in the relevant jurisdiction in which the License terms are sought to be enforced according to the corresponding provisions of the implementation of those treaty provisions in the applicable national law. If the standard suite of rights granted under applicable copyright law includes additional rights not granted under this License, such additional rights are deemed to be included in the License; this License is not intended to restrict the license of any rights under applicable law.