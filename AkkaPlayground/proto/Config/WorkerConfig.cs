using System.Collections.Generic;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Masking;

namespace AkkaPlayground.Proto.Config
{
    public abstract class WorkerConfig
    {
        public abstract Network BelongsTo { get; }

        public List<FieldName> Fields { get; protected set; }

        private FieldMask _fieldMask;
        public FieldMask FieldMask
        {
            get { return _fieldMask ??= new FieldMask(Fields); }
        }
    }
}