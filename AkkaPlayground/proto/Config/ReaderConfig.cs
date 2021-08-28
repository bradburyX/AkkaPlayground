using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Config
{
    public class ReaderConfig : WorkerConfig
    {
        public int IntervalSeconds { get; protected set; }
        public override Network BelongsTo => Network.Read;
    }
}