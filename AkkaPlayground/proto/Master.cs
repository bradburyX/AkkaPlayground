using Akka.Actor;
using Akka.Routing;
using AkkaPlayground.proto.actors;
using AkkaPlayground.proto.data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaPlayground.proto
{
    public class Master:ReceiveActor
    {
        private readonly Dictionary<Network,IActorRef> _networks 
            = new Dictionary<Network, IActorRef>();
        public Master()
        {
            _networks.Add(
                Network.Write,
                CreateNetwork(
                    new List<Props>
                    {
                        Props.Create(() => new Writer()),
                        //TODO: ActorFactory(string type, string instanceName, string config)
                        //TODO: get from config
                        Props.Create(() => new Writer()),
                        Props.Create(() => new Writer())
                    },
                    Network.Write
                )
            );
            _networks.Add(
                Network.Read,
                CreateNetwork(
                    new List<Props>
                    {
                        Props.Create(() => new Reader())
                    },
                    Network.Read
                )
            );
            Receive<Forward>(
                fwd =>
                {
                    Console.WriteLine($"Master got {fwd.Message}");
                    _networks[fwd.Network].Tell(fwd.Message);
                }
            );

        }

        private IActorRef CreateNetwork(
            List<Props> propsList,
            Network network
        )
        {
            var role = network.ToString();
            var brokers =
                propsList
                    .Select((p,i) =>
                        Context.ActorOf(
                            Props.Create(() => new MessageBroker(p)), 
                            $"{role}Chan{i}"
                        )
                    );
            return
                Context.ActorOf(
                    Props.Empty.WithRouter(
                        new BroadcastGroup(brokers.Select(b => b.Path.ToString()))
                    ),
                    $"{role}Network"
                );
        }
    }
}
