using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPlayground.proto.data
{
    public class MessageEnvelope<TMessage>
    {
        public MessageEnvelope(TMessage message, long messageId)
        {
            Message = message;
            MessageId = messageId;
        }

        public TMessage Message { get; private set; }

        public long MessageId { get; private set; }
    }
}
