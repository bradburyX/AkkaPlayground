using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Akka.Event;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaPlayground.Actors;
using AkkaPlayground.Data;
using AkkaPlayground.Graph;

namespace AkkaPlayground
{
    class Program
    {
        private static ActorSystem system = ActorSystem.Create("MySystem");

        static async Task Main(string[] args)
        {
            var actor = system.ActorOf<MyReceiveActor>("poorActor");
            system.EventStream.Subscribe(actor, typeof(string));

            var mySource = RestartSource
                .OnFailuresWithBackoff(
                    () =>
                    {
                        Console.WriteLine("Start/Restart...");
                        return NumbersSource.Create();
                    },
                    RestartSettings.Create(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        0.2
                    )
                );

            var materializer = ActorMaterializer.Create(system);
            await mySource
                //.Throttle(1, TimeSpan.FromSeconds(1), 1, ThrottleMode.Shaping)
                .RunForeach(
                    i => system.EventStream.Publish($"Broadcast to all faithful listeners: {i}!"), 
                    materializer
                );

            Console.ReadKey();
        }

        static void IEnumerableSource()
        {
            var sourceGraph = new NumbersSource();
            var mySource = Source.FromGraph(sourceGraph);
            var materializer = ActorMaterializer.Create(system);
            var result1Task = mySource.Take(10).RunAggregate(0, (sum, next) => sum + next, materializer);

            Console.WriteLine(result1Task.Result);
            

            Console.ReadKey();
        }

        static void events()
        {
            var monitorProps = Props.Create<DeadletterMonitor>(() => new DeadletterMonitor(system.EventStream));
            var monitor = system.ActorOf(monitorProps, "Reaper");


            var listener1 = system.ActorOf<MyReceiveActor>("listener1");
            var listener2 = system.ActorOf<MyReceiveActor>("listener2");

            
            //listener1.Tell(PoisonPill.Instance);
            system.EventStream.Subscribe(listener1, typeof(string));
            system.EventStream.Subscribe(listener2, typeof(string));
            system.EventStream.Subscribe(listener2, typeof(MyMessage));

            system.EventStream.Publish(new MyMessage("Hellloooooooooo ladies!"));
            system.EventStream.Publish("Hellloooooooooo boys!");
            
            Console.ReadKey();
        }

        static void deadLetters()
        {
            var monitorProps = Props.Create<DeadletterMonitor>(() => new DeadletterMonitor(system.EventStream));
            var monitor = system.ActorOf(monitorProps, "Reaper");
            system.EventStream.Subscribe(monitor, typeof(DeadLetter));


            var actor = system.ActorOf<MyReceiveActor>("poorActor");
            system.EventStream.Publish(new DeadLetter("hullabloob", actor,actor));
            // actor must DIE to produce deadLetters
            actor.Tell(PoisonPill.Instance);
            //actor.Tell(42);
            actor.Tell(false); // this will only become a dead letter on shutdown...
            actor.Tell("hello");

        }

        static void TestBasics()
        {
            var actor = system.ActorOf<MyUntypedActor>();
            actor.Tell("hello");

            var propsUntyped = Props.Create<MyUntypedActor>();
            var actor2 = system.ActorOf(propsUntyped, "anotherUntyped");
            actor2.Tell("hello too");

            var propsUntypedCtor = Props.Create<MyUntypedActor>(() => new MyUntypedActor(2));
            var actor3 = system.ActorOf(propsUntypedCtor, "ctor");

        }
    }
}
