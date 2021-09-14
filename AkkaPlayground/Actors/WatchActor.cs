using Akka.Actor;

namespace AkkaPlayground.Actors
{
    public class WatchActor : ReceiveActor
    {
        private IActorRef child = Context.ActorOf(Props.Empty, "child");
        private IActorRef lastSender = Context.System.DeadLetters;

        public WatchActor()
        {
            Context.Watch(child); // <-- this is the only call needed for registration

            Receive<string>(s => s.Equals("kill"), msg =>
            {
                Context.Stop(child);
                lastSender = Sender;
            });

            Receive<Terminated>(t => t.ActorRef.Equals(child), msg =>
            {
                lastSender.Tell("finished");
            });
        }
    }
}
