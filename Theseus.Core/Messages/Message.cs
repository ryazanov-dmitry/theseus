using System;

namespace Theseus.Core.Messages
{
    public class Message
    {
        public object MessageObject { get; set; }
        public object Sender { get; set; }

        public DateTime SentDateTime { get; set; }
    }
}