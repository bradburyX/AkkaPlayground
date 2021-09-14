using System;
using System.Collections.Generic;
using System.Text;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Factory
{
    public class ActorForAttribute:Attribute
    {
        public Network Network { get; }
        public RepositoryType RepositoryType { get; }

        public ActorForAttribute(Network network, RepositoryType repositoryType)
        {
            Network = network;
            RepositoryType = repositoryType;
        }
    }
}
