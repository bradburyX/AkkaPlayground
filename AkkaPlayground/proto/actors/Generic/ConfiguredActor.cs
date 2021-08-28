using Akka.Actor;
using AkkaPlayground.Proto.Config;

namespace AkkaPlayground.Proto.Actors.Generic
{
    public class ConfiguredActor : ReceiveActor
    {
        protected readonly BaseConfig BaseConfig;
        protected readonly WorkerConfig WorkerConfig;

        public ConfiguredActor(BaseConfig baseConfig, WorkerConfig workerConfig)
        {
            BaseConfig = baseConfig;
            WorkerConfig = workerConfig;
        }
    }
}
