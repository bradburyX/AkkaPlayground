using System;
using System.Collections.Generic;
using System.Text;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;

namespace AkkaPlayground.Proto.Actors.Specific
{
    public class MssqlReader : ConfiguredActor
    {
        public MssqlReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {

        }
    }
}
