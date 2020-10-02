using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Theseus.Core.Crypto;
using Theseus.Core.Dto;
using Theseus.Core.Messages;
using Xunit;
using Xunit.Abstractions;

namespace Theseus.Core.Tests
{
    public class DKGClientTests
    {
        private readonly Mock<IAuthentication> auth;
        private readonly ITestOutputHelper o;
        private AdHocSrwcService medium;

        public DKGClientTests(ITestOutputHelper o)
        {
            this.o = o;
            auth = new Mock<IAuthentication>();
            medium = new AdHocSrwcService(o);
        }

        [Fact]
        public async Task CreateDKGInitRequest_NoSessionFirstInGroupToTriggerDKG_ReceiveAcceptFromOtherNode()
        {
            //Arrange
            var initiatorOfDKG = CreateClient();
            var node2 = CreateClient();
            var medium = new AdHocSrwcService();
            medium.RegisterColleagues(new List<IDKGClient> { initiatorOfDKG, node2 });
            const string NodeId = "someNodeId";

            //Act
            await initiatorOfDKG.TryInitDKGSession(NodeId);

            //Assert
            // Assert.True(medium.GetLastMessage()) //TODO: Last message DTO with some status?
        }


        /// <summary>
        /// Lets try approach without persisting request history, so node just sends new TryInitDKGSession
        /// and 'master' node must handle this.
        /// </summary>
        [Fact]
        public void NodeReceivesBeaconAfterDKGSessionStart_NodeSendsTryInitDKGSession_MasterNodeResponds()
        {
            Assert.True(false);
            //Arrange
            //Act

            //Assert
        }

        /// <summary>
        /// Lets assume we can use central time synchronization service. Node will store send time in request.
        /// When node receives request it checks who was first and acts accordingly. For that node needs to persists its request time.
        /// </summary>
        [Fact]
        public async Task TryInitDKGSession_2NodesSendDKGStartSimultaneously_2NodeAccepts()
        {
            //Arrange
            const string proverNodeId = "proverNode";
            var node1MessageLog = new MessageLog();
            node1MessageLog.Log(proverNodeId, new Beacon());

            var node2MessageLog = new MessageLog();
            node2MessageLog.Log(proverNodeId, new Beacon());
            var node2Auth = Common.CreateAuth();
            node2MessageLog.Log(
                node2Auth.Base64PublicKey(),
                new DKGInitRequest
                {
                    NodeId = proverNodeId,
                    SentTime = DateTime.Now
                });


            var node1 = CreateClient(node1MessageLog);
            var node2 = CreateClient(node2MessageLog, node2Auth);

            medium.RegisterColleagues(new List<IDKGClient> { node1, node2 });

            //Act
            await node1.TryInitDKGSession(proverNodeId);

            //Assert
            Assert.Equal(2, medium.GetAllMessages().Count);
            Assert.True(medium.GetLastMessageFrom(node2) is DKGStartSessionAccept);

            Thread.Sleep(1000);
            Assert.True(medium.GetLastMessageFrom(node1) is DKGSessionList);
        }


        [Fact]
        public void ReceiveTryInitDKGSession_NotReadyForDKG_SendDecline()
        {
            Assert.True(false);
        }

        [Fact]
        public void MoreThan1ParallelDKGSessionStart()
        {
            Assert.True(false);
        }

        private IDKGClient CreateClient()
        {
            return CreateClient(new MessageLog(), Common.CreateAuth());
        }

        private IDKGClient CreateClient(MessageLog messageLog)
        {
            return CreateClient(messageLog, Common.CreateAuth());
        }


        private IDKGClient CreateClient(MessageLog messageLog, IAuthentication auth)
        {
            return new DKGClient(medium, auth, messageLog, new Session(), new Chronos());
        }
    }
}