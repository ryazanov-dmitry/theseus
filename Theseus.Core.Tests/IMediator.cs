using System.Collections.Generic;

namespace Theseus.Core.Tests
{
    internal interface IMediator
    {
        void RegisterColleagues(List<INode> colleagues);
    }

}