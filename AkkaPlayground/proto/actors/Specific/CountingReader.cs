using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Specific
{
    public class CountingReader: ConfiguredActor
    {
        private int i;

        public CountingReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            Receive<Reader.DoWorkCycle>(
                _ =>
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Context.Parent.Tell(
                            new DataRow(
                                WorkerConfig.FieldsMask,
                                (i++).ToString()
                            )
                        );
                    }
                });
        }
    }
}
