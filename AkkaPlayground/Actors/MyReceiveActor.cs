using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Akka.Actor;
using AkkaPlayground.Data;

namespace AkkaPlayground.Actors
{
    public class MyReceiveActor: ReceiveActor
    {
        public MyReceiveActor()
        {
            Receive<MyMessage>(
                message => Console.WriteLine($"OnReceive {this.Self.Path.Name}\nmessageObj: {message.Message}")
            );
            Receive<string>(
                message => Console.WriteLine($"OnReceive {this.Self.Path.Name}\nmessageTxt: {message}")
            );
            Receive<int>(
                message => Console.WriteLine($"OnReceive {this.Self.Path.Name}\nmessageNum: {message}")
            );
        }
        protected override void PreStart()
        {
            Console.WriteLine($"PreStart {this.Self.Path.Name}");
            base.PreStart();
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Console.WriteLine($"PreRestart {this.Self.Path.Name}\nreason: {JsonSerializer.Serialize(reason)}\nmessage: {JsonSerializer.Serialize(message)}");
            base.PreRestart(reason, message);
        }

        protected override void PostRestart(Exception reason)
        {
            Console.WriteLine($"PostRestart {this.Self.Path.Name}\nreason: {JsonSerializer.Serialize(reason)}");
            base.PostRestart(reason);
        }
    }
}
