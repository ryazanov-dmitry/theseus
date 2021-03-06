using System;
using System.Threading.Tasks;
using Moq;
using Theseus.Core;
using Theseus.Core.Tests;

namespace Theseus.Demo.Renderer
{
    class Program
    {
        private static FakeWorld world;
        private static ISrwcService medium;

        static void Main(string[] args)
        {
            world = new FakeWorld();
            medium = new Mock<ISrwcService>().Object;


            world.AddSubject(CreateSubject(SubjectType.Courier, 1));
            world.AddSubject(CreateSubject(SubjectType.Client, 10));

            var renderer = new Renderer(world);

            world.SimulateDKGReady().Wait();
            while (world.SomeoneIsMoving()) ;
            
            Console.ReadKey();
        }

        private static Subject CreateSubject(SubjectType subjectType, float coords)
        {
            var coordinates = new Coordinates { X = coords };
            var subject = new Subject
            {
                Coordinates = coordinates,
                SubjectType = subjectType
            };

            var fakeGps = new FakeGPS(coordinates);

            var fakeNavigator = new FakeNavigator(fakeGps, subject.Coordinates, world.Ticker);

            var node = new Node(medium, Common.CreateAuth(), fakeGps, null, null, null, fakeNavigator);
            var gateway = new FakeNodeGateway(node);
            subject.FakeNodeGateway = gateway;
            return subject;
        }
    }
}
