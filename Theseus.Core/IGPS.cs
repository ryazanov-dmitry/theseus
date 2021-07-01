namespace Theseus.Core
{
    public interface IGPS
    {
        Coordinates GetGPSCoords();
    }

    public class Coordinates
    {
        public int X { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Coordinates coordinates &&
                   X == coordinates.X;
        }
    }
}