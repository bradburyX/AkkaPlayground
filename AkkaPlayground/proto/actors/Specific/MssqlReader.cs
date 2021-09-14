using AkkaPlayground.Proto.Actors.Factory;
using AkkaPlayground.Proto.Actors.Generic;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Specific
{
    [ActorFor(Network.Read, RepositoryType.MSSQL)]
    public class MssqlReader : ConfiguredActor
    {
        public MssqlReader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {

        }
    }
}
