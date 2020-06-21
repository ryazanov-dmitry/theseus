using System.Collections.Generic;

namespace Theseus.Core.Tests
{
    internal interface IMediator
    {
        void RegisterColleagues(List<IColleagues> colleagues);
    }

    internal interface IColleagues
    {

    }
}