using AkkaPlayground.Proto.Data.Masking;
using Cassandra.Mapping.Attributes;

namespace AkkaPlayground.Proto.Data.Cassandra
{
    [Table("master")]
    public class DataField
    {
        [PartitionKey]
        public string Id { get; set; }

        [PartitionKey]
        [Column(Type = typeof(int))]
        public FieldName Col { get; set; }

        public string Val { get; set; }
    }

    public static class FieldExtension
    {
        public static DataField ToPersistentField(this Field self, string id)
        {
            return 
                new DataField
                {
                    Id = id,
                    Col = self.Col,
                    Val = self.Val
                };
        }
    }
}
