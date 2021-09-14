using System;
using AkkaPlayground.Proto.Actors.Factory;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Messaging;

namespace AkkaPlayground.Proto.Actors.Specific
{
    [ActorFor(Network.Write, RepositoryType.MSSQL)]
    public class MssqlWriter : ConfiguredActor
    {
        public MssqlWriter(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            Receive<MessageEnvelope<DataPackage>>(
                env =>
                {
                    var filteredContent =
                        new ChangeSet(
                            env.Message.Content.Id,
                            env.Message.Content.Filter(workerConfig.FieldMask)
                        );
                    Console.WriteLine($"Write to MSSQL {baseConfig.Name}: {filteredContent}");
                    Sender.Tell(new Confirmation(env.MessageId), Self);
                });
        }
    }
}
