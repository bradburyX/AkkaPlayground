using System;
using AkkaPlayground.Proto.Actors.Factory;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Messaging;

namespace AkkaPlayground.Proto.Actors.Specific
{
    [ActorFor(Network.Write, RepositoryType.CSV)]
    public class CsvWriter : ConfiguredActor
    {
        public CsvWriter(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            Receive<MessageEnvelope<DataPackage>>(
                env =>
                {
                    //Thread.Sleep(1000);
                    Console.WriteLine($"Write to CSV: {env.Message.Content}");
                    Sender.Tell(new Confirmation(env.MessageId), Self);
                });
        }
    }
}
