using System;
using System.Collections.Generic;

namespace Theseus.Core.Dto
{
    public class DKGRequest : SignedObject
    {
        public string NodeId { get; set; }

        /// <summary>
        /// DKGRequest also contains GPS coordinates
        /// claimed by customer. This is needed to protect service from malicious customer. For
        /// example, if some peer which receives DKGRequest with GPS coordinates pointing to
        /// the peers area and this peer doesn’t receive customers beacon, then peer will send
        /// warning message to the service.
        /// 
        /// TODO: For now just int to simplify.
        /// </summary>
        /// <value></value>
        public float GPSCoordinates { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DKGRequest request &&
                   EqualityComparer<byte[]>.Default.Equals(Signature, request.Signature) &&
                   EqualityComparer<byte[]>.Default.Equals(Key, request.Key) &&
                   NodeId == request.NodeId &&
                   GPSCoordinates == request.GPSCoordinates;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Signature, Key, NodeId, GPSCoordinates);
        }
    }
}