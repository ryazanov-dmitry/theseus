using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Theseus.Core;
using Xunit;

namespace Theseus.Demo.Tests
{
    public class FakeWorldTests
    {
        private readonly FakeWorld _world;

        public FakeWorldTests()
        {
            this._world = new FakeWorld();
        }

        [Fact]
        public void SetupVerifierNodeOn1DPlane()
        {
            //When
            _world.AddSubject(new Subject { SubjectType = SubjectType.Verifier, Coordinates = new Coordinates { X = 6 } });

            //Then
            Assert.True(_world.Nodes.Single() is INodeGateway);
        }

        [Fact]
        public void FakeWorldCanSeeMovingOfNode()
        {
            //Given
            const int clientCoords = 3;

            _world.AddSubject(new Subject { SubjectType = SubjectType.Courier, Coordinates = new Coordinates { X = 1 } });
            _world.AddSubject(new Subject { SubjectType = SubjectType.Client, Coordinates = new Coordinates { X = clientCoords } });

            //When
            _world.TriggerClient();
            while(_world.SomeoneIsMoving);

            //Then
            var coords = _world.GetCoords();
            var courierCoords = coords.Single(x => x.SubjectType == SubjectType.Courier).Coordinates;

            Assert.Equal(clientCoords, courierCoords);
        }
    }

    internal interface INodeGateway
    {
        void Receive(object message);
    }

    internal class Subject
    {
        public Coordinates Coordinates { get; internal set; }
        public SubjectType SubjectType { get; internal set; }
    }

    internal enum SubjectType
    {
        Verifier,
        Courier,
        Client
    }

    internal class FakeWorld
    {
        public FakeWorld()
        {
        }

        public List<INodeGateway> Nodes { get; } = new List<INodeGateway>();
        public bool SomeoneIsMoving { get; internal set; }

        internal void AddSubject(Subject subject)
        {
            switch (subject.SubjectType)
            {
                case SubjectType.Verifier:
                    Nodes.Add(CreateNode(subject.Coordinates));
                    break;
                default:; break;
            }
        }

        internal List<Subject> GetCoords()
        {
            throw new NotImplementedException();
        }

        internal void TriggerClient()
        {
            throw new NotImplementedException();
        }

        private INodeGateway CreateNode(Coordinates coordinates)
        {
            var mock = new Mock<IGPS>();
            mock.Setup(x => x.GetGPSCoords()).Returns(new Coordinates
            {
                X = coordinates.X
            });
            var node = new Node(null, null, mock.Object, null, null, null);
            var gateway = new FakeNodeGateway(node);
            return gateway;
        }
    }

    internal class FakeNodeGateway : INodeGateway
    {
        private INode node;

        public FakeNodeGateway(INode node)
        {
            this.node = node;
        }

        public void Receive(object message)
        {
            throw new NotImplementedException();
        }
    }

    internal class VerifierInitParams
    {
        public Coordinates Coords { get; internal set; }
    }
}
