using Akka.Actor;
using Akka.Dispatch;

namespace AkkaPlayground.Proto
{
    public class TestStablePriorityMailbox : UnboundedStablePriorityMailbox
    {
        public TestStablePriorityMailbox(Settings settings, Akka.Configuration.Config config) : base(settings, config)
        {
        }

        protected override int PriorityGenerator(object message)
        {
            return 1;
        }
    }
}
