namespace Theseus.Core
{
    public interface IGPS
    {
        Coordinates GetGPSCoords();
    }

    public class Coordinates
    {
        public int DummyCoords { get; set; }
    }
}