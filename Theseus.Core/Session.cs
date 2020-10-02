using System;
using System.Collections.Generic;

namespace Theseus.Core
{
    public interface ISession
    {
        void Add(Guid sessionId, DKGSession dkgSession);
        DKGSession Get(Guid sessionId);
    }


    public class Session : ISession
    {
        private readonly Dictionary<Guid, DKGSession> dictionary;
        
        public Session()
        {
            dictionary = new Dictionary<Guid, DKGSession>();
        }

        public void Add(Guid sessionId, DKGSession dkgSession)
        {
            dictionary.Add(sessionId, dkgSession);
        }

        public DKGSession Get(Guid sessionId)
        {
            return dictionary[sessionId];
        }
    }
}