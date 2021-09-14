namespace AkkaPlayground.Proto.Data.Messaging
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
