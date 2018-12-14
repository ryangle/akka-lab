﻿using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Common;

namespace SeedNode1
{
    public class ServiceActor : ReceiveActor
    {
        public const string BinaryFunction = "BinaryFunction";
        public ServiceActor()
        {
            Receive<ServiceDiscovery>(msg =>
            {
                var binaryFunction = Context.Child(BinaryFunction);
                if (binaryFunction.IsNobody())
                {
                    binaryFunction = Context.ActorOf<BinaryFunctionActor>(BinaryFunction);
                }
                binaryFunction.Forward(msg);
            });
        }
    }
}