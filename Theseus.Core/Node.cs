using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theseus.Core
{
    public class Node
    {
        public string Id { get; }

        private readonly ISrwcService srwcService;

        public Node(ISrwcService srwcService)
        {
            this.Id = GenerateId();
            this.srwcService = srwcService;
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task BroadcastPersonalBeacon()
        {
            await srwcService.Broadcast("node 1 beacon");
        }

        public Task RequestDKG(string id)
        {
            throw new NotImplementedException();
        }

        public List<string> GetDKGPubs()
        {
            throw new NotImplementedException();
        }
    }
}