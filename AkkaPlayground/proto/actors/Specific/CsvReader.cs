using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Specific
{
    public class CsvReader: ConfiguredActor
    {
        public CsvReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            
        }
    }
}
