using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theseus.Core.Dto;

namespace Theseus.Demo
{
    public class FakeWorld
    {
        public Ticker Ticker { get; set; }

        public FakeWorld()
        {
            Ticker = new Ticker();
        }

        public List<Subject> Subjects { get; } = new List<Subject>();

        public void AddSubject(Subject subject)
        {
            Subjects.Add(subject);
        }

        public async Task SimulateDKGReady()
        {
            var courier = Subjects.Single(x => x.SubjectType == SubjectType.Courier);
            var client = Subjects.Single(x => x.SubjectType == SubjectType.Client);

            var deliveryRequest = new DeliveryRequest
            {
                GPSCoordinates = client.Coordinates.X,
                NodeId = client.FakeNodeGateway.Node.Id
            };

            await courier.FakeNodeGateway.Node.ReceiveDeliveryRequest(deliveryRequest);

            var dkgPub = new DKGPub { };
            courier.FakeNodeGateway.Node.ReceiveDKGPublicKey(dkgPub);
        }

        public bool SomeoneIsMoving()
        {
            return Subjects.Any(x => x.IsMoving());
        }

        internal void TriggerClient()
        {
            throw new NotImplementedException();
        }


    }
}