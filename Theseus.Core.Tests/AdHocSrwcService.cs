using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Theseus.Core.Dto;
using Theseus.Core.Infrastructure;
using Theseus.Core.Messages;
using Xunit.Abstractions;

namespace Theseus.Core.Tests
{
    /// <summary>
    /// Fake short range wireless communication service.
    /// </summary>
    public class AdHocSrwcService : ISrwcService, IMediator
    {

        private readonly ITestOutputHelper o;

        public List<FakeNodeGateway> colleagues;
        public readonly IMessageLog messageLog;

        public AdHocSrwcService()
        {
            this.messageLog = new MessageLog();
        }

        public AdHocSrwcService(ITestOutputHelper o)
        {
            this.messageLog = new MessageLog();
            this.o = o;
        }

        public async Task Broadcast(object message, object sender = null)
        {
            if (o != null)
            {
                o.WriteLine(JsonConvert.SerializeObject(message));
            }

            messageLog.Log(sender, message);

            foreach (var node in this.colleagues.Where(x => x != sender && x.Node != sender && x.Node.DKGClient != sender))
            {
                await node.Receive(message);
            }

        }

        public void RegisterColleagues(List<FakeNodeGateway> colleagues)
        {
            this.colleagues = colleagues;
        }


        internal object GetLastMessageFrom(object sender)
        {
            return messageLog.Get<object>(sender).LastOrDefault();
        }

        internal List<object> GetAllMessages()
        {
            return messageLog.GetAll();
        }
    }
}