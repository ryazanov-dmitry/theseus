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
    }
}