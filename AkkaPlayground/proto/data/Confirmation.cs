namespace AkkaPlayground.Proto.Data
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
