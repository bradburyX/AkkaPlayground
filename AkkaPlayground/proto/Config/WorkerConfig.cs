using System.Collections.Generic;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Config
{
    public abstract class WorkerConfig
    {
        public abstract Network BelongsTo { get; }

        public List<Fields> Fields { get; protected set; }

        private FieldsMask _fieldsMask;
        public FieldsMask FieldsMask
        {
            get { return _fieldsMask ??= new FieldsMask(Fields); }
        }
    }
}