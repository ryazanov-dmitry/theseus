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
        private readonly float _signalRange = 3;
        private readonly bool _considerCoordinates = false;

        public AdHocSrwcService(bool considerCoordinates = false, bool writeToSeparateConsole = false)
        {
            this._considerCoordinates = considerCoordinates;
            this.messageLog = new MessageLog(writeToSeparateConsole);
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

            var recepients = this.colleagues
                                    .Where(x => x != sender && x.Node != sender && x.Node.DKGClient != sender);

            if(_considerCoordinates)
                recepients = recepients.Where(x => IsInRange(sender, x));

            foreach (var node in recepients)
            {
                await node.Receive(message);
            }

        }

        private bool IsInRange(object sender, FakeNodeGateway potentialReceiver)
        {
            var senderNode = this.colleagues.Single(x => x.Node == sender || x.Node.DKGClient == sender);

            var distance = Geometry.Distance(
                senderNode.Node.GetCoordinates().X,
                potentialReceiver.Node.GetCoordinates().X);

            return distance <= _signalRange;
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