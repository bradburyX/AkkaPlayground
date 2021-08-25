using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Akka.Actor;
using AkkaPlayground.proto.data;

namespace AkkaPlayground.proto.actors
{
    public class Writer : ReceiveActor
    {
        public Writer()
        {
            Receive<MessageEnvelope<Message>>(
                env =>
                {
                    //Thread.Sleep(1000);
                    Console.WriteLine($"Write to repository: {env.Message.Content}");
                    Sender.Tell(new Confirmation(env.MessageId), Self);
                });

        }

        protected override void PreStart()
        {
            // repo iniz here!
            base.PreStart();
        }

        protected override void PostStop()
        {
            // repo dispose here!
            base.PostStop();
        }
    }
}
