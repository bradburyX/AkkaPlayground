using AkkaPlayground.Proto.Data.Masking;

namespace AkkaPlayground.Proto.Data.Messaging
{
    public class DataPackage
    {
        public DataPackage(ChangeSet content)
        {
            Content = content;
            FieldMask = new FieldMask(content.Fields.GetFieldNames());
        }
        public DataPackage(ChangeSet content, string exclusiveRecipient)
        {
            Content = content;
            FieldMask = new FieldMask(content.Fields.GetFieldNames());
            ExclusiveRecipient = exclusiveRecipient;
        }

        public DataPackage(ChangeSet content, FieldMask fieldMask)
        {
            FieldMask = fieldMask;
            Content = content;
        }

        public string ExclusiveRecipient { get; private set; }
        public ChangeSet Content { get; private set; }
        public FieldMask FieldMask { get; set; }
    }
}
