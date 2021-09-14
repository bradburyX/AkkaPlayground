using Akka.Actor;
using AkkaPlayground.Proto.Actors.Specific;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AkkaPlayground.Proto.Actors.Factory;

namespace AkkaPlayground.Proto
{
    public class WorkerFactory
    {
        private static Dictionary<(Network, RepositoryType), Type> _typeLookup;

        private static Dictionary<(Network, RepositoryType), Type> TypeLookup
        {
            get
            {
                return _typeLookup ??=
                    typeof(TestReader)
                        .Assembly
                        .GetTypes()
                        .Select(t =>
                            (
                                t, 
                                a: (ActorForAttribute)t
                                    .GetCustomAttributes(typeof(ActorForAttribute), false)
                                    .FirstOrDefault()
                            )
                        )
                        .Where(r => r.a != null)
                        .ToDictionary(t => (t.a.Network, t.a.RepositoryType), t => t.t);
            }
        }

        public static Props Provide(BaseConfig info, WorkerConfig workerConfig)
        {
            if (!TypeLookup.TryGetValue((workerConfig.BelongsTo, info.Type), out var type))
            {
                throw new ArgumentOutOfRangeException($"RepoType {info.Type} not supported for {workerConfig.BelongsTo}");
            }
            return Props.Create(type, info, workerConfig);
        }
    }
}