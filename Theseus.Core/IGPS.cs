namespace Theseus.Core
{
    public interface IGPS
    {
        Coordinates GetGPSCoords();
    }

    public class Coordinates : ICom
    {
        public int X { get; set; }
    }
}