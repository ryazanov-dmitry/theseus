namespace Theseus.Core.Dto
{

    // This will be send by TCP/IP so SingedObject probably is not right way to check signature...
    // TODO: Design how Node will receive DeliveryRequest via TCP/IP.
    public class DeliveryRequest
    {
        public string NodeId { get; set; }

        public float GPSCoordinates { get; set; }
    }
}