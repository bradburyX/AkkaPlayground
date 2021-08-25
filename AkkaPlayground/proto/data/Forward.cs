using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPlayground.proto.data
{
    public class Forward
    {
        public Forward(Network network, Message message)
        {
            Network = network;
            Message = message;
        }

        public Network Network { get; }
        public Message Message { get; }
    }
}
