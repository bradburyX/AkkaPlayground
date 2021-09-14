using System.Collections.Generic;
using System.Linq;
using AkkaPlayground.Proto.Data.Masking;

namespace AkkaPlayground.Proto.Data
{
    public class Field
    {
        public FieldName Col { get; set; }
        public string Val { get; set; }
    }

    public static class FieldListExtensions
    {
        public static List<FieldName> GetFieldNames(this List<Field> self)
        {
            return self.Select(f => f.Col).ToList();
        }
    }
}
