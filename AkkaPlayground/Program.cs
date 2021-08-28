using Akka.Actor;
using Akka.Configuration;
using Akka.Event;
using Akka.Routing;
using Akka.Streams;
using Akka.Streams.Dsl;
using AkkaPlayground.Actors;
using AkkaPlayground.Data;
using AkkaPlayground.Graph;
using AkkaPlayground.Proto;
using AkkaPlayground.Proto.Actors;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using AkkaPlayground.Proto.Actors.Generic;

namespace AkkaPlayground
{
    /*
     things:
     ActorRefs.NoSender
     */

    class Program
    {
        private static ActorSystem system = 
            ActorSystem.Create("MySystem", ConfigurationFactory.ParseString(GetConfig()));

        private static string GetConfig()
        {
            return @"
akka.actor.default-dispatcher.throughput = 100  #ensure we process 100 messages per mailbox run
stable-prio-mailbox{
    mailbox-type : """ + typeof(TestStablePriorityMailbox).AssemblyQualifiedName + @"""
}";

            return @"
akka.actor.default-dispatcher.throughput = 100  #ensure we process 100 messages per mailbox run
stable-prio-mailbox{
    mailbox-capacity = 1000
    mailbox-push-timeout-time = 10s
    mailbox-type : ""Akka.Dispatch.BoundedMailbox, Akka""
}
";
        }

        static void Main()
        {
            var config = new RepositoryConfigCollection();
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build()
                .GetSection("Repositories")
                .Bind(config, options =>
                {
                    options.BindNonPublicProperties = true;
                });

            var result = config.CheckIntegrity();
            if (!result.IsSuccess)
            {
                Console.WriteLine(result.Exception.Message);
                return;
            }

            system.ActorOf(
                Props.Create(() => new Master(config)),
                "master"
            );

            Console.ReadKey();
        }
        static void proto(string[] args)
        {
            //var writer = Props.Create(() => new Writer());//.WithMailbox("stable-prio-mailbox");
            /*
            // constructor removed...
            system.ActorOf(
                Props.Create(() => new MessageBroker(writer)),
                "channel1"
            );
            system.ActorOf(
                Props.Create(() => new MessageBroker(writer)),
                "channel2"
            );
            system.ActorOf(
                Props.Create(() => new MessageBroker(writer)),
                "channel3"
            );
            */
            
            var brokers = 
                new[]
                {
                    "/user/channel1",
                    //"/user/channel2",
                    //"/user/channel3"
                
                };
            var router = 
                system.ActorOf(
                    Props.Empty.WithRouter(new RoundRobinGroup(brokers)), 
                    "reveiverNetwork"
                );

            for (int i = 0; i < 8; i++)
            {
                system
                    .ActorSelection("akka://MySystem/user/reveiverNetwork")
                    .Tell(new Broadcast(new DataRow($"{i}")));
                //router.Tell(new Broadcast(new Message($"{i}")));
            }

/*            
            //config?
            var config = ConfigurationFactory.ParseString(
@"
routees.paths = [
    ""akka://MySystem/user/Worker1"" #testing full path
    user/Worker2
    user/Worker3
    user/Worker4
]");
            var roundRobinGroup = system.ActorOf(Props.Empty.WithRouter(new RoundRobinGroup(config)));
*/
            Console.ReadKey();
        }

        public async void CustomEventStreamDistribution()
        {
            var actor = system.ActorOf<MyReceiveActor>("poorActor");
            actor.Tell("Welcome to the stage!");
            // restart actor if down?
            // -> resend event if not ack'd?
            system.EventStream.Subscribe(actor, typeof(string));

            var monitorProps = Props.Create<DeadletterMonitor>(() => new DeadletterMonitor(system.EventStream));
            var monitor = system.ActorOf(monitorProps, "Reaper");

            var mySource = RestartSource
                .OnFailuresWithBackoff(
                    () => Source.FromGraph(new NumbersSource()),
                    RestartSettings.Create(
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(3),
                        0.2
                    )
                );

            var materializer = ActorMaterializer.Create(system);
            await mySource
                //.Throttle(1, TimeSpan.FromSeconds(1), 1, ThrottleMode.Shaping) // to slow down
                .RunForeach(
                    i =>
                    {
                        Console.WriteLine($"Sending {i}");
                        system.EventStream.Publish($"Broadcast to all faithful listeners: {i}!");
                    }, 
                    materializer
                );
        }

        public void IEnumerableSource()
        {
            var sourceGraph = new NumbersSource();
            var mySource = Source.FromGraph(sourceGraph);
            var materializer = ActorMaterializer.Create(system);
            var result1Task = mySource.Take(10).RunAggregate(0, (sum, next) => sum + next, materializer);

            Console.WriteLine(result1Task.Result);
            

            Console.ReadKey();
        }

        public void Events()
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

        public void DeadLetters()
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

        public void TestBasics()
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
