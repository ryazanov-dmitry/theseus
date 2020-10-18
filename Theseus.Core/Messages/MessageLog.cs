using System;
using System.Collections.Generic;
using System.Linq;

namespace Theseus.Core.Messages
{
    public class MessageLog : IMessageLog
    {

        private readonly List<Message> messages;

        public MessageLog()
        {
            this.messages = new List<Message>();
        }

        public List<TMessage> Get<TMessage>(object from, int lastMinutes)
        {
            return messages.Where(x => from != null ? x.Sender == from : true)
                           .Where(x => x.SentDateTime > DateTime.Now.AddMinutes(-lastMinutes))
                           .Where(x => x.MessageObject is TMessage)
                           .Select(x => (TMessage)x.MessageObject)
                           .ToList();
        }

        public List<TMessage> Get<TMessage>(object sender)
        {
            return Get<TMessage>(sender, 3600);
        }

        public List<TMessage> Get<TMessage>(int lastMinutes)
        {
            return Get<TMessage>(null, lastMinutes);
        }

        public List<object> GetAll()
        {
            return messages.Select(x => x.MessageObject).ToList();
        }

        public void Log(object sender, object message)
        {
            this.messages.Add(new Message
            {
                MessageObject = message,
                Sender = sender,
                SentDateTime = DateTime.Now
            });
        }
    }

    public interface IMessageLog
    {
        void Log(object sender, object message);
        List<TMessage> Get<TMessage>(object sender, int lastMinutes);
        List<TMessage> Get<TMessage>(object sender);
        List<TMessage> Get<TMessage>(int lastMinutes);
        List<object> GetAll();
    }
}