using AkkaPlayground.proto.data;

namespace AkkaPlayground.proto.actors
{
    using Akka.Actor;
    using System;

    public class MessageBroker :MessageBrokerBase
    {
        private int i;
        private static int globalNum;
        private int myNum = globalNum++;

        private class StartWorking { }

        private readonly IActorRef _worker;

        public MessageBroker(Props props)
        {
            _worker = Context.ActorOf(props, $"{Self.Path.Name}Worker");
            BaseLogic();

            Command<Message>(
                message => IsForMe(message) && IsMatch(message),
                message =>
                {
                    MaybeTriggerException();
                    Console.WriteLine($"Will relay message {message.Content}");
                    Deliver(_worker.Path, messageId => new MessageEnvelope<Message>(message, messageId));
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
        protected override void Unhandled(object message) { }

        private bool IsMatch(Message message)
        {
            return string.IsNullOrEmpty(message.ExclusiveRecipient) || message.ExclusiveRecipient == Self.Path.Name;
        }

        private bool IsForMe(Message message)
        {
            return
                string.IsNullOrEmpty(message.AnswerTo)
                || message.AnswerTo == _worker.Path.Name;
        }


        private void MaybeTriggerException()
        {
            return; //nope
            if (i++ == 7)
            {
                Console.WriteLine($"aaaaargh");
                throw new Exception("I'm dying!");
            }
        }

    }
}
