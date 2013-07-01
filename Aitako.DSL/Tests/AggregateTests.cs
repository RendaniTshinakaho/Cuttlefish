using System;
using Aitako.DSL.Components;
using Microsoft.VisualStudio.TextTemplating;
using NUnit.Framework;

namespace Aitako.DSL.Tests
{
    [TestFixture]
    public class AggregateTests
    {
        [SetUp]
        public void Setup()
        {
            Make.InTheBoundedContextOf("Test");
            Make.WithNamespace("Test");
        }

        private const string NAME = "Person";

        [Test]
        public void CanAddCommands()
        {
            DomainObjectGeneratorBase person = Aggregate.AggregateNamed(NAME)
                                                        .ThatCan("CapturePerson")
                                                        .WithParameter<Guid>("Id")
                                                        .WithParameter<string>("Firstname")
                                                        .WithParameter<string>("Lastname")
                                                        .And()
                                                        .ThatCan("ChangeLastname")
                                                        .WithParameter<string>("Lastname")
                                                        .Done();

            Assert.That(person.Commands.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanAddEvents()
        {
            DomainObjectGeneratorBase person = Aggregate.AggregateNamed(NAME)
                                                        .ThatRaises("PersonCaptured")
                                                        .WithParameter<Guid>("Id")
                                                        .WithParameter<string>("Firstname")
                                                        .WithParameter<string>("Lastname")
                                                        .Done();

            Assert.That(person.Events.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanAddFields()
        {
            DomainObjectGeneratorBase person = Aggregate.AggregateNamed(NAME)
                                                        .WithField<string>("Firstname")
                                                        .WithField<string>("Lastname");

            Assert.That(person.Fields.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanGenerateFile()
        {
            DomainObjectGeneratorBase person = Aggregate.AggregateNamed(NAME)
                                                        .WithField<string>("Firstname")
                                                        .WithField<string>("Lastname")
                                                        .WithField<Guid>("SomeId")
                                                        .ThatCan("CapturePerson")
                                                        .WithParameter<Guid>("Id")
                                                        .WithParameter<string>("Firstname")
                                                        .WithParameter<string>("Lastname")
                                                        .And()
                                                        .ThatRaises("PersonCaptured")
                                                        .WithParameter<Guid>("Id")
                                                        .WithParameter<string>("Firstname")
                                                        .WithParameter<string>("Lastname")
                                                        .Done();

            var template = Activator.CreateInstance<AggregateGenerator>();
            var session = new TextTemplatingSession();
            session["Service"] = person;
            template.Session = session;
            //template.Initialize();
            Console.WriteLine(template.TransformText());
        }

        [Test]
        public void CanInstantiateAggregate()
        {
            Aggregate person = Aggregate.AggregateNamed(NAME);

            Assert.That(person, Is.Not.Null);
            Assert.That(person.Name, Is.EqualTo(NAME + "Aggregate"));
        }

        [Test]
        public void CanSetParentClass()
        {
            DomainObjectGeneratorBase person = Aggregate.AggregateNamed(NAME).ChildOf("Test").Done();

            Assert.That(person.Parent, Is.EqualTo("Test"));
        }
    }
}