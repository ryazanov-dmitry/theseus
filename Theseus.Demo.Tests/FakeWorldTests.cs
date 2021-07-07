using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Theseus.Core;
using Theseus.Core.Dto;
using Theseus.Core.Infrastructure;
using Theseus.Core.Navigation;
using Theseus.Core.Tests;
using Xunit;
using Xunit.Abstractions;

namespace Theseus.Demo.Tests
{
    public class FakeWorldTests
    {
        private readonly FakeWorld _world;
        private readonly ITestOutputHelper output;
        private readonly ISrwcService medium;

        public FakeWorldTests(ITestOutputHelper output)
        {
            this._world = new FakeWorld();
            this.output = output;
            medium = new Mock<ISrwcService>().Object;
        }


        [Fact]
        public async Task FakeWorldCanSeeMovingOfNode()
        {
            //Given
            var clientCoords = 3;

            _world.AddSubject(CreateSubject(SubjectType.Courier, 1));
            _world.AddSubject(CreateSubject(SubjectType.Client, clientCoords));

            //When
            await _world.SimulateDKGReady();
            while (_world.SomeoneIsMoving()) ;

            //Then
            var courierCoords = _world.Subjects.Single(x => x.SubjectType == SubjectType.Courier).Coordinates.X;

            Assert.Equal(clientCoords, courierCoords);
        }

        private Subject CreateSubject(SubjectType subjectType, float coords)
        {
            var coordinates = new Coordinates { X = coords };
            var subject = new Subject
            {
                Coordinates = coordinates,
                SubjectType = subjectType
            };

            var fakeGps = new FakeGPS(coordinates);

            var fakeNavigator = new FakeNavigator(fakeGps, subject.Coordinates, _world.Ticker);

            var node = new Node(medium, Common.CreateAuth(), fakeGps, null, null, null, fakeNavigator);
            var gateway = new FakeNodeGateway(node);
            subject.FakeNodeGateway = gateway;
            return subject;
        }
    }

    internal class VerifierInitParams
    {
        public Coordinates Coords { get; internal set; }
    }
}
