﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AkkaPlayground.proto.data
{
    public enum Fields
    {
        Name = 1,
        Email = 2,
        Address = 3,
        City = 4,
        Country = 5,
        Birthdate = 6,
        Office = 7,
        Salary = 8,
        Job = 9,
        AcademicTitles = 10,
        SvNr = 11,
        Website = 12,
        HairColor = 13,
        Nationality = 14,
        StarSign = 15,
        Status = 16,
        Smoker = 17
    }

    public class FieldsMask
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

        public Fields[] Fields { get; }
        public ushort[] Mask { get; }

        private const int MaskSize = 16;

        private static readonly Dictionary<Fields, FieldBitmaskAtIndex> FieldList =
            Enum
                .GetValues(typeof(Fields))
                .Cast<Fields>()
                .Select((f, i) =>
                    new { f, i }
                )
                .ToDictionary(
                    k => k.f,
                    v => new FieldBitmaskAtIndex(v.i)
                );

        private static readonly int MaskLength = (int)Math.Ceiling((double)FieldList.Count/ MaskSize);

        public FieldsMask(params Fields[] fields)
        {
            Fields = fields;
            Mask = new ushort[MaskLength];
            foreach (var field in fields)
            {
                var key = FieldList[field];
                Mask[key.Index] = (ushort)(Mask[key.Index] | key.Mask);
            }
        }

        public bool IsMatch(FieldsMask compare)
        {
            for (var i = 0; i < MaskSize; i++)
            {
                if ((Mask[i] & compare.Mask[i]) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        public Fields[] Intersect(FieldsMask b)
        {
            return Fields.Intersect(b.Fields).ToArray();
        }
    }
}