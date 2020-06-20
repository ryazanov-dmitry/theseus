using System;
using System.Collections.Generic;

namespace Theseus.Core.Tests
{
    internal class FakeEther
    {
        private List<Node> lists;

        public FakeEther(List<Node> lists)
        {
            this.lists = lists;
        }

        internal object GetLastReceivedMessage()
        {
            throw new NotImplementedException();
        }
    }
}