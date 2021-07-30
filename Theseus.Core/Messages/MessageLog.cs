using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Theseus.Core.Messages
{
    public class MessageLog : IMessageLog
    {

        private readonly List<Message> messages;

        public MessageLog()
        {
            this.messages = new List<Message>();
        }
        private readonly bool _writeToSeparateConsole = false;
        private StreamWriter sw;

        public MessageLog(bool writeToSeparateConsole)
        {
            this._writeToSeparateConsole = writeToSeparateConsole;
            this.messages = new List<Message>();
        }

        public List<TMessage> Get<TMessage>(object from, int lastMinutes)
        {
            return messages.Where(x => from != null ? x.Sender.Equals(from) : true)
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
            var log = new Message
            {
                MessageObject = message,
                SentDateTime = DateTime.Now
            };

            if (_writeToSeparateConsole)
                WriteToConsole(log);

            log.Sender = sender;

            this.messages.Add(log);
        }

        private void WriteToConsole(Message log)
        {
            if (sw == null)
            {
                Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "/bin/bash";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardInput = true;
                proc.Start();

                sw = proc.StandardInput;
                // StreamReader sr = p.StandardOutput;
            }
            var jsonString = JsonConvert.SerializeObject(log);
            
            sw.WriteLine(jsonString);
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