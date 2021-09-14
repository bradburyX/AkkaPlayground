using System.Collections.Generic;
using System.Linq;
using AkkaPlayground.Proto.Data.Masking;

namespace AkkaPlayground.Proto.Data
{
    public class ChangeSet
    {
        public ChangeSet(string id, List<Field> fields)
        {
            Id = id;
            Fields = fields;
        }
        public string Id { get; }

        //public Dictionary<FieldName,string> Fields { get; }
        public List<Field> Fields { get; set; }

        public List<Field> Filter(FieldMask mask)
        {
            return
                Fields
                    .Where(f => mask.Fields.Contains(f.Col))
                    .ToList();
        }

        public override string ToString()
        {
            return
                Id + " - "+
                Fields
                    .Select(kvp => $"{kvp.Col}: {kvp.Val}")
                    .Aggregate((s, s1) => s + " - " + s1);
        }
    }
}
