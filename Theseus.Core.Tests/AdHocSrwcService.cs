using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Theseus.Core.Dto;
using Theseus.Core.Messages;
using Xunit.Abstractions;

namespace Theseus.Core.Tests
{
    /// <summary>
    /// Fake short range wireless communication service.
    /// </summary>
    internal class AdHocSrwcService : ISrwcService, IMediator
    {
        private List<IDKGClient> dkgClients;

        private readonly ITestOutputHelper o;

        public List<INode> colleagues;
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

        /// <summary>
        /// This is dummy Broadcast. It imitates that srwc middleware classifies messages, which implies
        /// Node's interface will be called with particular method like ReceiveDKG or ReceiveBeacon.
        /// I guess its right...?
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>

        public async Task Broadcast(object message, object sender = null)
        {
            if (o != null)
            {
                o.WriteLine(JsonConvert.SerializeObject(message));
            }

            messageLog.Log(sender, message);

            if (message is DKGRequest dKGRequest)
            {
                foreach (var node in this.colleagues.Where(x => x != sender))
                {
                    await node.ReceiveDKG(dKGRequest);
                }
            }

            if (message is Beacon beacon)
            {
                foreach (var node in this.colleagues.Where(x => x != sender))
                {
                    node.ReceiveBeacon(beacon);
                }
            }

            if (message is DKGInitRequest dkgInitRequest)
            {
                foreach (var dkgClient in dkgClients.Where(x => x != sender))
                {
                    await dkgClient.ReceiveTryInitDKGSession(dkgInitRequest);
                }
            }

            if (message is DKGStartSessionAccept dkgStartSessionAccept)
            {
                foreach (var dkgClient in dkgClients.Where(x => x != sender))
                {
                    await dkgClient.ReceiveDKGStartSessionAccept(dkgStartSessionAccept);
                }
            }

            if (message is DKGSessionList dkgSessionList)
            {
                foreach (var dkgClient in dkgClients.Where(x => x != sender))
                {
                    // await dkgClient.ReceiveDKGStartSessionAccept(dkgStartSessionAccept);
                }
            }
        }

        public void RegisterColleagues(List<INode> colleagues)
        {
            this.colleagues = colleagues;
        }

        public void RegisterColleagues(List<IDKGClient> colleagues)
        {
            this.dkgClients = colleagues;
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