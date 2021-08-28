using System;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Config
{
    public class RepositoryConfig
    {
        public BaseConfig Info { get; protected set; }
        public ReaderConfig Reader { get; protected set; }
        public WriterConfig Writer { get; protected set; }
        
        public WorkerConfig GetForRole(Network role)
        {
            switch (role)
            {
                case Network.Read:
                    return Reader;
                case Network.Write:
                    return Writer;
                default:
                    throw new ArgumentOutOfRangeException(nameof(role), role, null);
            }
        }
    }
}
