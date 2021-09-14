namespace AkkaPlayground.Proto.Data.Messaging
{
    public class Forward
    {
        public Forward(Network network, DataPackage message)
        {
            Network = network;
            Message = message;
        }

        public Network Network { get; }
        public DataPackage Message { get; }
    }
}
