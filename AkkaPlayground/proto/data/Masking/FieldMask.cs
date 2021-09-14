using System;
using System.Collections.Generic;
using System.Linq;

namespace AkkaPlayground.Proto.Data.Masking
{
    public class FieldMask
    {
        public struct FieldBitmaskAtIndex
        {
            public FieldBitmaskAtIndex(int pos)
            {
                Index = pos / MaskSize;
                Mask = (ushort)(1 << (MaskSize - 1 - pos % MaskSize));
            }

            public int Index { get; set; }
            public ushort Mask { get; set; }
        }

        public FieldName[] Fields { get; }
        public ushort[] Mask { get; }

        private const int MaskSize = 16;

        private static FieldMask _matchAll;
        public static FieldMask MatchAll
        {
            get
            {
                return
                    _matchAll
                        ??=
                        new FieldMask(
                            Enum.GetValues(typeof(FieldName)).Cast<FieldName>().ToList()
                        );
            }
        }

        private static readonly Dictionary<FieldName, FieldBitmaskAtIndex> FieldList =
            Enum
                .GetValues(typeof(FieldName))
                .Cast<FieldName>()
                .Select((f, i) =>
                    new { f, i }
                )
                .ToDictionary(
                    k => k.f,
                    v => new FieldBitmaskAtIndex(v.i)
                );

        private static readonly int MaskLength = 
            (int)Math.Ceiling((double)FieldList.Count / MaskSize);


        public FieldMask()
        {
            Fields = new FieldName[0];
            Mask = new ushort[MaskLength];
        }
        public FieldMask(List<FieldName> fields)
        {
            Fields = fields.ToArray();
            Mask = new ushort[MaskLength];
            foreach (var field in fields)
            {
                var key = FieldList[field]; // could be list[enumValue] instead of dict
                Mask[key.Index] = (ushort)(Mask[key.Index] | key.Mask);
            }
        }

        public FieldMask(FieldName field)
        {
            Mask = new ushort[MaskLength];
            var key = FieldList[field]; // could be list[enumValue] instead of dict
            Mask[key.Index] = (ushort) (Mask[key.Index] | key.Mask);
        }

        public bool IsMatch(FieldMask compare)
        {
            if (compare == null)
            {
                return false;
            }
            for (var i = 0; i < MaskLength; i++)
            {
                if ((Mask[i] & compare.Mask[i]) != 0)
                {
                    return true;
                }
            }

            return false;
        }
        public FieldName[] Intersect(FieldMask b)
        {
            return Fields.Intersect(b.Fields).ToArray();
        }
    }
}