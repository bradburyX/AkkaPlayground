using System;
using Akka.Actor;
using AkkaPlayground.Data;

namespace AkkaPlayground.Actors
{
    public class MyReceiveActor: ReceiveActor
    {
        public MyReceiveActor()
        {
            Receive<MyMessage>(
                message => Console.WriteLine($"OnReceive {Self.Path.Name}\nmessageObj: {message.Message}")
            );
            Receive<string>(
                message =>
                {
                    Console.WriteLine($"OnReceive {Self.Path.Name}\nmessageTxt: {message}");
                    //if(message.Contains("42"))
                    //    throw new Exception(); // what about the this and the lost msg afterwards? retry?
                    //Thread.Sleep(2000);
                });
            Receive<int>(
                message => Console.WriteLine($"OnReceive {Self.Path.Name}\nmessageNum: {message}")
            );
        }
        protected override void PreStart()
        {
            Console.WriteLine($"PreStart {Self.Path.Name}");
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine($"PreRestart {Self.Path.Name}\nreason: {reason.Message}\nmessage: {message}");
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            Console.WriteLine($"PostRestart {Self.Path.Name}\nreason: {reason.Message}");
            base.PostRestart(reason);
        }
    }
}
