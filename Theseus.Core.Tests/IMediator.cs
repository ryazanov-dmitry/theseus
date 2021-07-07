using System.Collections.Generic;
using Theseus.Core.Infrastructure;

namespace Theseus.Core.Tests
{
    internal interface IMediator
    {
        void RegisterColleagues(List<FakeNodeGateway> colleagues);
    }

}