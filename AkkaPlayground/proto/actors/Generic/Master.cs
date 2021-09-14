using System;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Messaging;

namespace AkkaPlayground.Proto.Actors.Generic
{
    // TODO LOGGING
    public class Master : ReceiveActor
    {
        public Master(RepositoryConfigCollection config)
        {
            CreateNetwork(config, Network.Read);
            CreateNetwork(config, Network.Write);

            Receive<Forward>(
                fwd =>
                {
                    Console.WriteLine($"Master got {fwd.Message.Content}");
                    Context
                        .ActorSelection(fwd.Network.ToString())
                        .Tell(fwd.Message);
                }
            );

        }

        private void CreateNetwork(RepositoryConfigCollection config, Network network)
        {
            var brokers = config
                .Select(c =>
                    new
                    {
                        c.Info, worker = c.GetForRole(network)
                    }
                )
                .Where(c => c.worker != null)
                .Select(c =>
                    Context.ActorOf(
                        Props.Create(() => new MessageBroker(c.Info, c.worker)),
                        $"{network}{c.Info.Name}"
                    )
                )
                .ToList();

            Context.ActorOf(
                Props.Empty.WithRouter(
                    new BroadcastGroup(brokers.Select(b => b.Path.ToString()))
                ),
                network.ToString()
            );
        }
    }
}
