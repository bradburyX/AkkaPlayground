using System;
using Akka.Actor;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;

namespace AkkaPlayground.Proto.Actors.Generic
{
    public class Reader : ConfiguredActor
    {
        private readonly ReaderConfig _workerConfig;
        private ICancelable _workSchedule;
        private readonly IActorRef _worker;

        public class DoWorkCycle { }

        public Reader(BaseConfig baseConfig, WorkerConfig workerConfig)
            :base(baseConfig,workerConfig)
        {
            _workerConfig = (ReaderConfig)workerConfig;
            /* TODO:
             * worker gets kicked to action (go-msg to worker)
             * spews out line by line (or IEnumerable?) his whole data (or changed if it knows...)
             *      irrelevant to know when or if worker's finished
             * reader has dedicated mini db
             * is data in db (missing OR different)?
             *      send out diff to parent
             *      update db
             */
            _worker = Context.ActorOf(
                WorkerFactory.Provide(baseConfig, workerConfig),
                $"{workerConfig.BelongsTo}{baseConfig.Name}Repo"
            );

            // this we get from the worker
            Receive<DataRow>(
                msg =>
                {
                    // check if data is actually changed..
                    Context.Parent.Tell(
                        new Forward(Network.Write, msg)
                    );
                });
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

            // repo iniz here!
            base.PreStart();
        }

        protected override void PostStop()
        {
            _workSchedule?.Cancel();
            // repo dispose here!
            base.PostStop();
        }
    }
}
