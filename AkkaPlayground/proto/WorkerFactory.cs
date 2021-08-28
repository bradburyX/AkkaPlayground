using Akka.Actor;
using AkkaPlayground.Proto.Actors.Specific;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using System;
using System.Collections.Generic;

namespace AkkaPlayground.Proto
{
    public class WorkerFactory
    {
        private static Dictionary<(Network, RepositoryType), Type> typeLookup =
            new Dictionary<(Network, RepositoryType), Type>
            {
                { (Network.Read,  RepositoryType.Counting), typeof(CountingReader) },
                { (Network.Read,  RepositoryType.CSV),      typeof(CsvReader)      },
                { (Network.Read,  RepositoryType.MSSQL),    typeof(MssqlReader)    },
                { (Network.Write, RepositoryType.CSV),      typeof(CsvWriter)      },
                { (Network.Write, RepositoryType.MSSQL),    typeof(MssqlWriter)    }
            };
        public static Props Provide(BaseConfig info, WorkerConfig workerConfig)
        {
            if (!typeLookup.TryGetValue((workerConfig.BelongsTo, info.Type), out var type))
            {
                throw new ArgumentOutOfRangeException($"RepoType {info.Type} not supported for {workerConfig.BelongsTo}");
            }
            return Props.Create(type, info, workerConfig);
        }
    }
}