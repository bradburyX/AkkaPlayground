using System;
using System.Linq;
using Akka.Actor;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Cassandra;
using AkkaPlayground.Proto.Data.Messaging;

namespace AkkaPlayground.Proto.Actors.Generic
{
    public class Reader : ConfiguredActor
    {
        private readonly ReaderConfig _workerConfig;
        private ICancelable _workSchedule;
        private readonly IActorRef _worker;
        private CassandraRepo<DataField> _cassandraRepo;

        public class DoWorkCycle { }

        public Reader(BaseConfig baseConfig, WorkerConfig workerConfig)
            : base(baseConfig, workerConfig)
        {
            _workerConfig = (ReaderConfig)workerConfig;
            _worker = Context.ActorOf(
                WorkerFactory.Provide(baseConfig, workerConfig),
                $"{workerConfig.BelongsTo}{baseConfig.Name}Repo"
            );

            Receive<ChangeSet>(
                row =>
                {
                    var changes = GetChangedFieldsForRow(row);
                    if (!changes.Fields.Any())
                        return;
                    Context.Parent.Tell(
                        new Forward(
                            Network.Write,
                            new DataPackage(changes)
                        )
                    );
                    PersistChangedFields(changes);
                }
            );
        }

        private ChangeSet GetChangedFieldsForRow(ChangeSet changeSet)
        {
            var fields = changeSet.Fields.GetFieldNames().Cast<int>();
            var persisted =
                _cassandraRepo
                    .Load(f => fields.Contains((int)f.Col) && f.Id == changeSet.Id)
                    .ToList();
            var changes =
                changeSet.Fields
                    .Where(c =>
                        persisted.All(p => p.Col != c.Col) ||
                        persisted.First(p => p.Col == c.Col).Val != c.Val
                    )
                    .ToList();
            return new ChangeSet(changeSet.Id, changes);
        }
        private void PersistChangedFields(ChangeSet changes)
        {
            _cassandraRepo.Insert(
                changes
                    .Fields
                    .Select(field => field.ToPersistentField(changes.Id))
            );
        }

        protected override void PreStart()
        {
            _workSchedule =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(_workerConfig.IntervalSeconds),
                    _worker,
                    new DoWorkCycle(),
                    Self
                );
            _cassandraRepo = new CassandraRepo<DataField>();
            base.PreStart();
        }

        protected override void PostStop()
        {
            _workSchedule?.Cancel();
            _cassandraRepo.Dispose();
            base.PostStop();
        }
    }
}
