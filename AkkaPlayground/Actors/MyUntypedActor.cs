using System;
using System.Text.Json;
using Akka.Actor;

namespace AkkaPlayground.Actors
{
    public class MyUntypedActor:UntypedActor
    {
        public MyUntypedActor()
        {
            
        }

        public MyUntypedActor(int magicNumber)
        {
            Console.WriteLine($"Ctor {Self.Path.Name}\nmagicNumber: {magicNumber}");
        }

        protected override void OnReceive(object message)
        {
            Console.WriteLine($"OnReceive {Self.Path.Name}\nmessage: {JsonSerializer.Serialize(message)}");
        }
        protected override void PreStart()
        {
            Console.WriteLine($"PreStart {Self.Path.Name}");
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine($"PreRestart {Self.Path.Name}\nreason: {JsonSerializer.Serialize(reason)}\nmessage: {JsonSerializer.Serialize(message)}");
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            Console.WriteLine($"PostRestart {Self.Path.Name}\nreason: {JsonSerializer.Serialize(reason)}");
            base.PostRestart(reason);
        }


        public static Props Props(int magicNumber)
        {
            return Akka.Actor.Props.Create(() => new MyUntypedActor(magicNumber));
        }
    }
}
