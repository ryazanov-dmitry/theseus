using Theseus.Core;
using Theseus.Core.Navigation;
using Theseus.Core.Tests;

namespace Theseus.Demo
{
    public class Subject : IMovable
    {
        public Coordinates Coordinates { get; set; }
        public SubjectType SubjectType { get; set; }
        public FakeNodeGateway FakeNodeGateway { get; set; }

        public bool IsMoving()
        {
            return FakeNodeGateway.Node.IsMoving();
        }
    }
}