using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPlayground.Data
{
    public class MyMessage
    {
        public string Message { get; set; }

        public MyMessage(string message)
        {
            Message = message;
        }
    }
}
