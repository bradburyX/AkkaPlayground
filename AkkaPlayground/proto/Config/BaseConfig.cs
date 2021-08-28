namespace AkkaPlayground.Proto.Config
{
    public class BaseConfig
    {
        public string Name { get; protected set; }
        public RepositoryType Type { get; protected set; }
        public string Connection { get; protected set; }
    }
}