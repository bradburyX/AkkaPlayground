using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPlayground.proto.data
{
    public class Confirmation
    {
        public Confirmation(long messageId)
        {
            MessageId = messageId;
        }

        public long MessageId { get; private set; }
    }
}
