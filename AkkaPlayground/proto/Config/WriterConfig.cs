using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Config
{
    public class WriterConfig:WorkerConfig
    {
        public override Network BelongsTo => Network.Write;
    }
}