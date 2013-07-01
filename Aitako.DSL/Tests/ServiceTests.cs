using System;
using Aitako.DSL.Components;
using Microsoft.VisualStudio.TextTemplating;
using NUnit.Framework;

namespace Aitako.DSL.Tests
{
    [TestFixture]
    public class ServiceTests
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
            DomainObjectGeneratorBase service = Service.ServiceNamed(NAME)
                                                       .ThatCan("CapturePerson")
                                                       .WithParameter<Guid>("Id")
                                                       .WithParameter<string>("Firstname")
                                                       .WithParameter<string>("Lastname")
                                                       .And()
                                                       .ThatCan("ChangeLastname")
                                                       .WithParameter<string>("Lastname")
                                                       .Done();

            Assert.That(service.Commands.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanAddEvents()
        {
            DomainObjectGeneratorBase service = Service.ServiceNamed(NAME)
                                                       .ThatRaises("PersonCaptured")
                                                       .WithParameter<Guid>("Id")
                                                       .WithParameter<string>("Firstname")
                                                       .WithParameter<string>("Lastname")
                                                       .Done();

            Assert.That(service.Events.Count, Is.EqualTo(1));
        }

        [Test]
        public void CanAddFields()
        {
            DomainObjectGeneratorBase service = Service.ServiceNamed(NAME)
                                                       .WithField<string>("Firstname")
                                                       .WithField<string>("Lastname");

            Assert.That(service.Fields.Count, Is.EqualTo(2));
        }

        [Test]
        public void CanGenerateFile()
        {
            DomainObjectGeneratorBase service = Service.ServiceNamed(NAME)
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
            session["Service"] = service;
            template.Session = session;
            Console.WriteLine(template.TransformText());
        }

        [Test]
        public void CanInstantiateAggregate()
        {
            Service service = Service.ServiceNamed(NAME);

            Assert.That(service, Is.Not.Null);
            Assert.That(service.Name, Is.EqualTo(NAME + "Service"));
        }
    }
}