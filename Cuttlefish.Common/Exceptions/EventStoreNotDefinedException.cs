﻿using System;

namespace Cuttlefish
{
    public class EventStoreNotDefinedException : Exception
    {
        public EventStoreNotDefinedException()
            : base("No event store has been defined. Please make sure that you define one before calling 'Done'") { }
    }
}