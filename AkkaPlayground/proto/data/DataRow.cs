namespace AkkaPlayground.Proto.Data
{
    public class DataRow
    {
        public DataRow(string content)
        {
            Content = content;
        }
        public DataRow(string content, string exclusiveRecipient)
        {
            Content = content;
            ExclusiveRecipient = exclusiveRecipient;
        }

        public DataRow(FieldsMask fieldsMask, string content)
        {
            FieldsMask = fieldsMask;
            Content = content;
        }

        public string ExclusiveRecipient { get; private set; }
        public string Content { get; private set; }
        public FieldsMask FieldsMask { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }
}
