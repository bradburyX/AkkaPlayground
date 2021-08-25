using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Persistence;
using AkkaPlayground.proto.data;

namespace AkkaPlayground.proto.actors
{
    public class MessageBrokerBase : AtLeastOnceDeliveryReceiveActor
    {
        public MessageBrokerBase()
        {
            BaseLogic();
        }

        protected void BaseLogic()
        {
            Command<Confirmation>(ack =>
            {
                ConfirmDelivery(ack.MessageId);
            });
            // recover the most recent at least once delivery state
            Recover<SnapshotOffer>(
                offer => offer.Snapshot is Akka.Persistence.AtLeastOnceDeliverySnapshot,
                offer =>
                {
                    var snapshot = offer.Snapshot as Akka.Persistence.AtLeastOnceDeliverySnapshot;
                    SetDeliverySnapshot(snapshot);
                }
            );
            Command<CleanSnapshots>(clean =>
            {
                // save the current state (grabs confirmations)
                SaveSnapshot(GetDeliverySnapshot());
            });
            Command<SaveSnapshotSuccess>(saved =>
            {
                var seqNo = saved.Metadata.SequenceNr;
                // delete all but the most current snapshot
                DeleteSnapshots(
                    new SnapshotSelectionCriteria(seqNo, saved.Metadata.Timestamp.AddMilliseconds(-1))
                );
            });
            // all is well
            Command<DeleteSnapshotsSuccess>(delete => { });
            // log or do something else
            Command<SaveSnapshotFailure>(failure => { });
        }

        private class CleanSnapshots { }

        public override string PersistenceId => Context.Self.Path.Name;
        private ICancelable _recurringSnapshotCleanup;
        protected override void PreRestart(Exception reason, object message)
        {
            // retrieve crashed msg
            Stash.Stash();
            Stash.Unstash();
            base.PreRestart(reason, message);
        }
        protected override void PreStart()
        {
            DeleteSnapshots(SnapshotSelectionCriteria.Latest);
            _recurringSnapshotCleanup =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(10), Self, new CleanSnapshots(), ActorRefs.NoSender);
            base.PreStart();
        }
        protected override void PostStop()
        {
            _recurringSnapshotCleanup?.Cancel();
            base.PostStop();
        }
    }
}
