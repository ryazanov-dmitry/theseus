using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Messages;

namespace Theseus.Core
{

    public interface IDKGClient
    {
        Task TryInitDKGSession(string nodeId);
        Task ReceiveTryInitDKGSession(DKGInitRequest dKGStartRequest);
        Task ReceiveDKGStartSessionAccept(DKGStartSessionAccept dkgStartSessionAccept);
        Task SendAccept(Guid dKGStartRequest);
        Task ReceiveDKGSessionList(DKGSessionList dkgSessionList);
    }

    public class DKGClient : IDKGClient
    {
        private readonly ISrwcService srwcService;
        private readonly IAuthentication auth;
        private readonly IMessageLog messageLog;
        private readonly ISession session;
        private readonly IChronos chronos;
        private readonly IDKGCommunicator communicator;

        public DKGClient(
            ISrwcService srwcService,
            IAuthentication auth,
            IMessageLog messageLog,
            ISession session,
            IChronos chronos,
            IDKGCommunicator communicator)
        {
            this.srwcService = srwcService;
            this.auth = auth;
            this.messageLog = messageLog;
            this.session = session;
            this.chronos = chronos;
            this.communicator = communicator;
        }

        public async Task ReceiveTryInitDKGSession(DKGInitRequest dKGStartRequest)
        {
            auth.Verify(dKGStartRequest);
            LogIncommingMessage(dKGStartRequest);

            if (NoBeaconReceived(dKGStartRequest))
            {
                await SendDecline();
                return;
            }

            if (CurrentDidnotInitOrWasNotFirst(dKGStartRequest))
            {
                await SendAccept(dKGStartRequest.SessionId);
                return;
            }
        }

        public async Task ReceiveDKGStartSessionAccept(DKGStartSessionAccept dkgStartSessionAccept)
        {
            auth.Verify(dkgStartSessionAccept);

            var dkgSession = session.Get(dkgStartSessionAccept.SessionId);
            dkgSession.Participants.Add(dkgStartSessionAccept.Base64Key());
        }

        /// <summary>
        /// This method is for initiating DKG generation procedure.
        /// Node needs to know who will participate or ask to join to existing session.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public async Task TryInitDKGSession(string nodeId)
        {
            var dkgStartRequest = CreateDKGInitRequest(nodeId);
            CreateSessionAndSetTimeout(dkgStartRequest);
            await srwcService.Broadcast(dkgStartRequest, this);
            LogOutcommingMessage(dkgStartRequest);
        }

        public async Task SendAccept(Guid dkgSessionId)
        {
            var dkgStartSessionAccept = CreateDKGStartSessionAccept(dkgSessionId);
            await srwcService.Broadcast(dkgStartSessionAccept, this);
            LogOutcommingMessage(dkgStartSessionAccept);
        }

        private async Task SendDKGSessionList(Guid sessionId)
        {
            var dkgSession = session.Get(sessionId);
            var list = CreateDKGSessionList(dkgSession);

            await srwcService.Broadcast(list, this);
            LogOutcommingMessage(list);
        }

        private Task SendDecline()
        {
            throw new NotImplementedException();
        }

        private void CreateSessionAndSetTimeout(DKGInitRequest dkgInitRequest)
        {
            var dkgSession = new DKGSession
            {
                SessionId = dkgInitRequest.SessionId,
                ProverNodeId = dkgInitRequest.NodeId,
            };
            session.Add(dkgInitRequest.SessionId, dkgSession);

            var participantListCommit = new Task(async () =>
            {
                await SendDKGSessionList(dkgInitRequest.SessionId);
            });

            chronos.SetTimeout(participantListCommit, 3);
        }

        private bool CurrentDidnotInitOrWasNotFirst(DKGInitRequest dKGStartRequest)
        {
            var currentNodeRequest = messageLog
                .Get<DKGInitRequest>(auth.Base64PublicKey())
                .LastOrDefault(x => x.NodeId == dKGStartRequest.NodeId);

            //TODO: what if equal?
            return currentNodeRequest == null ? true : dKGStartRequest.SentTime < currentNodeRequest.SentTime;
        }

        private bool NoBeaconReceived(DKGInitRequest dKGStartRequest)
        {
            return !messageLog
                .Get<Beacon>(dKGStartRequest.NodeId, Node.BeaconValidTime)
                .Any();
        }

        private DKGInitRequest CreateDKGInitRequest(string nodeId)
        {
            var request = new DKGInitRequest
            {
                SessionId = Guid.NewGuid(),
                NodeId = nodeId,
                SentTime = DateTime.Now
            };
            auth.Sign(request);
            return request;
        }

        private DKGSessionList CreateDKGSessionList(DKGSession dkgSession)
        {
            var list = new DKGSessionList
            {
                SessionId = dkgSession.SessionId,
                Participants = dkgSession.Participants
            };
            auth.Sign(list);
            return list;
        }

        private DKGStartSessionAccept CreateDKGStartSessionAccept(Guid sessionId)
        {
            var request = new DKGStartSessionAccept
            {
                SessionId = sessionId
            };
            auth.Sign(request);
            return request;
        }

        private void LogOutcommingMessage(object dkgStartRequest)
        {
            messageLog.Log(auth.Base64PublicKey(), dkgStartRequest);
        }

        private void LogIncommingMessage(SignedObject message)
        {
            messageLog.Log(message.Base64Key(), message);
        }

        public Task ReceiveDKGSessionList(DKGSessionList dkgSessionList)
        {
            throw new NotImplementedException();
        }
    }

    public class DKGSession
    {
        public DKGSession()
        {
            Participants = new List<string>();
        }

        public Guid SessionId { get; set; }
        public string ProverNodeId { get; set; }
        public List<string> Participants { get; }
    }

    public class DKGInitRequest : SignedObject
    {
        public Guid SessionId { get; set; }
        public string NodeId { get; set; }
        public DateTime SentTime { get; set; }
    }

    public class DKGStartSessionAccept : SignedObject
    {
        public Guid SessionId { get; set; }
    }

    public class DKGSessionList : SignedObject
    {
        public Guid SessionId { get; set; }
        public List<string> Participants { get; set; }
    }
}