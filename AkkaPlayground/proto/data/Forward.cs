namespace AkkaPlayground.Proto.Data
{
    public class Forward
    {
        public Forward(Network network, DataRow message)
        {
            Network = network;
            Message = message;
        }

        public Network Network { get; }
        public DataRow Message { get; }
    }
}
