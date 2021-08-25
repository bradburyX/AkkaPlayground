using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Configuration;
using Akka.Dispatch;

namespace AkkaPlayground.proto
{
    public class TestStablePriorityMailbox : UnboundedStablePriorityMailbox
    {
        public TestStablePriorityMailbox(Settings settings, Config config) : base(settings, config)
        {
        }

        protected override int PriorityGenerator(object message)
        {
            return 1;
        }
    }
}
