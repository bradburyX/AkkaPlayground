using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Event;

namespace AkkaPlayground.Actors
{
    public class DeadletterMonitor : ReceiveActor
    {

        public DeadletterMonitor(EventStream systemEventStream)
        {
            systemEventStream.Subscribe(this.Self, typeof(DeadLetter));
            Receive<DeadLetter>(HandleDeadletter);
        }

        private void HandleDeadletter(DeadLetter dl)
        {
            Console.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
        }
    }

}
