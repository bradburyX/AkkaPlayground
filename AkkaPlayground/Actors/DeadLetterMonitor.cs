using Akka.Actor;
using Akka.Event;
using System;

namespace AkkaPlayground.Actors
{
    public class DeadletterMonitor : ReceiveActor
    {

        public DeadletterMonitor(EventStream systemEventStream)
        {
            systemEventStream.Subscribe(Self, typeof(DeadLetter));
            Receive<DeadLetter>(HandleDeadletter);
        }

        private void HandleDeadletter(DeadLetter dl)
        {
            Console.WriteLine($"DeadLetter captured: {dl.Message}, sender: {dl.Sender}, recipient: {dl.Recipient}");
        }
    }

}
