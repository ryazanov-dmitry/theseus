using System;
using System.Threading.Tasks;

namespace Theseus.Core
{
    public class Node
    {
        private readonly ISrwcService srwcService;

        public Node(ISrwcService srwcService)
        {
            this.srwcService = srwcService;
        }
        public async Task BroadcastPersonalBeacon()
        {
            await srwcService.Broadcast("node 1 beacon");
        }
    }
}