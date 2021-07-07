using Theseus.Core;

namespace Theseus.Demo
{
    public class FakeGPS : IGPS
    {
        private readonly Coordinates coords;
        public FakeGPS(Coordinates coords)
        {
            this.coords = coords;

        }
        public Coordinates GetGPSCoords()
        {
            return coords;
        }
    }
}