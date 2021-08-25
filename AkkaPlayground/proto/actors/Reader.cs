using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using AkkaPlayground.proto.data;

namespace AkkaPlayground.proto.actors
{
    public class Reader : ReceiveActor
    {
        private int i;
        private ICancelable _workSchedule;
        public class DoWorkCycle { }
        public Reader()
        {
            // receives steup info, hast separate step, bla
            Receive<DoWorkCycle>(
                env =>
                {
                    Console.WriteLine($"back to work!");
                    Context.Parent.Tell(
                        new Forward(
                            Network.Write,
                            new Message(
                                (i++).ToString()
                            )
                        )
                    );
                });
        }

        protected override void PreStart()
        {
            _workSchedule =
                Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(1),
                    Self, 
                    new DoWorkCycle(), 
                    ActorRefs.NoSender
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
