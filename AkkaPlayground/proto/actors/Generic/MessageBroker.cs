using System;
using Akka.Actor;
using AkkaPlayground.Proto.Config;
using AkkaPlayground.Proto.Data;
using AkkaPlayground.Proto.Data.Messaging;

namespace AkkaPlayground.Proto.Actors.Generic
{
    public class MessageBroker : MessageBrokerBase
    {
        private class StartWorking { }

        public MessageBroker(BaseConfig baseConfig, WorkerConfig workerConfig)
        {
            BaseLogic();

            var worker = CreateWorker(baseConfig, workerConfig);

            Command<DataPackage>(
                message => 
                    IsForMe(message) 
                    && IsMatch(message, workerConfig),
                message =>
                {
                    Console.WriteLine($"Will relay message {message.Content}");
                    Deliver(worker.Path, messageId => new MessageEnvelope<DataPackage>(message, messageId));
                    SaveSnapshot(GetDeliverySnapshot());
                }
            );

            Command<Forward>(
                forward =>
                {
                    Console.WriteLine($"Will forward message to master: {forward.Message}");
                    Context.Parent.Tell(forward);
                }
            );
        }

        private static IActorRef CreateWorker(BaseConfig baseConfig, WorkerConfig workerConfig)
        {
            if (workerConfig.BelongsTo == Network.Read)
            {
                // reader is generic, goes to factory himself to get his specific worker
                return Context.ActorOf(
                    Props.Create(() => new Reader(baseConfig, workerConfig)),
                    $"{workerConfig.BelongsTo}{baseConfig.Name}Diff"
                );
            }

            // writer is specific and stupid, doesn't need any abstraction
            return Context.ActorOf(
                WorkerFactory.Provide(baseConfig, workerConfig),
                $"{workerConfig.BelongsTo}{baseConfig.Name}Repo"
            );
        }

        protected override void PreStart()
        {
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(5),
                Self,
                new StartWorking(),
                ActorRefs.NoSender
            );
            base.PreStart();
        }

        // neccessary because:
        // broadcast seems to REALLY want to deliver the messsage
        // and doesn't like filters, they produce deadLetters :(
        protected override void Unhandled(object message) { }

        private bool IsMatch(DataPackage message, WorkerConfig workerConfig)
        {
            return workerConfig.FieldMask.IsMatch(message.FieldMask);
        }

        private bool IsForMe(DataPackage message)
        {
            return
                string.IsNullOrEmpty(message.ExclusiveRecipient)
                || message.ExclusiveRecipient == Self.Path.Name;
        }
    }
}
