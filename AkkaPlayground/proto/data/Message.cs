using System;
using System.Collections.Generic;
using System.Text;

namespace AkkaPlayground.proto.data
{
    public class Message
    {
        public Message(string content)
        {
            Content = content;
        }
        public Message(string content, string exclusiveRecipient)
        {
            Content = content;
            ExclusiveRecipient = exclusiveRecipient;
        }

        public string ExclusiveRecipient { get; private set; }
        public string Content { get; private set; }
        public string AnswerTo { get; set; }

        public override string ToString()
        {
            return Content;
        }
    }
}
