using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Generic
{
    public class Master : ReceiveActor
    {
        public Master(RepositoryConfigCollection config)
        {
            CreateNetwork(config, Network.Read);
            CreateNetwork(config, Network.Write);

            Receive<Forward>(
                fwd =>
                {
                    Console.WriteLine($"Master got {fwd.Message}");
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
                .Where(_ => _.worker != null)
                .Select(_ =>
                    Context.ActorOf(
                        Props.Create(() => new MessageBroker(_.Info, _.worker)),
                        $"{network}{_.Info.Name}"
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
