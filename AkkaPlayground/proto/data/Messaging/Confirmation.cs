namespace AkkaPlayground.Proto.Data.Messaging
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
