using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Akka;
using Akka.Streams;
using Akka.Streams.Stage;
using Akka.Streams.Dsl;

namespace AkkaPlayground.Graph
{
    public class NumbersSource : GraphStage<SourceShape<int>>
    {
        // Define the (sole) output port of this stage 
        public Outlet<int> Out { get; } = new Outlet<int>("NumbersSource");

        // Define the shape of this stage, which is SourceShape with the port we defined above
        public override SourceShape<int> Shape => new SourceShape<int>(Out);

        //this is where the actual logic will be created
        protected override GraphStageLogic CreateLogic(Attributes inheritedAttributes) 
            => new NumberLogic(this);
    }

    internal sealed class NumberLogic : GraphStageLogic
    {
        private readonly Outlet<int> _outlet;
        private readonly Queue<int> _queue = new Queue<int>();

        public NumberLogic(NumbersSource source) : base(source.Shape)
        {
            _outlet = source.Out;
            SetHandler(_outlet, TryPush);
        }

        public override void PreStart()
        {
            base.PreStart();

            var processCallback = GetAsyncCallback<int>(OnProcessEvent);

            Timer timer = new Timer {Interval = 1000};
            timer.Elapsed +=
                (sender, args) =>
                {
                    processCallback(DateTime.Now.Second);
                };
            timer.Start();
        }

        private void TryPush()
        {
            if (!IsAvailable(_outlet)) 
                return;
            if (!_queue.TryDequeue(out var msg))
                return;
            Push(_outlet, msg);
        }

        private void OnProcessEvent(int message)
        {
            //if(message % 15  == 0)throw new Exception(); // will get restarted, yay!
            _queue.Enqueue(message);
            TryPush();
        }
    }
}
