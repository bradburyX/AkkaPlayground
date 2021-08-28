using System;
using System.Collections.Generic;
using System.Text;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Specific
{
    public class MssqlWriter : ConfiguredActor
    {
        public MssqlWriter(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            Receive<MessageEnvelope<DataRow>>(
                env =>
                {
                    //Thread.Sleep(1000);
                    Console.WriteLine($"Write to MSSQL: {env.Message.Content}");
                    Sender.Tell(new Confirmation(env.MessageId), Self);
                });
        }
    }
}
