using System;
using System.Collections.Generic;
using System.Text;
using AkkaPlayground.Proto.Actors.Factory;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Specific
{
    public class TestReader : ConfiguredActor
    {
        [ActorFor(Network.Read, RepositoryType.Test)]
        public TestReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
           
        }
    }
}
