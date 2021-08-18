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
        protected override GraphStageLogic CreateLogic(Attributes inheritedAttributes) => new Logic(this);

        public static Source<int, NotUsed> Create()
        {
            return Source.FromGraph(new NumbersSource());
        }
    }

    internal sealed class Logic : GraphStageLogic
    {
        private readonly NumbersSource _source;
        private readonly Queue<int> _queue = new Queue<int>();

        public Logic(NumbersSource source) : base(source.Shape)
        {
            _source = source;
            SetHandler(_source.Out, TryPush);
        }

        public override void PreStart()
        {
            base.PreStart();
            Console.WriteLine($"PreStart source");
            var processCallback = GetAsyncCallback<int>(OnProcessEvent);

            Timer timer = new Timer {Interval = 1000};
            timer.Elapsed += 
                (sender, args) => { processCallback(DateTime.Now.Second); };
            timer.Start();
        }

        private void TryPush()
        {
            if (!IsAvailable(_source.Out)) 
                return;
            if (!_queue.TryDequeue(out var msg))
                return;
            Push(_source.Out, msg);
        }

        private void OnProcessEvent(int message)
        {
            //if(message % 15  == 0)throw new Exception(); // will get restarted!
            _queue.Enqueue(message);
            TryPush();
        }
    }
}
