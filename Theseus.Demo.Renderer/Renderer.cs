using System.Linq;

namespace Theseus.Demo.Renderer
{
    public class Renderer
    {
        private readonly FakeWorld world;
        private readonly float padding = 8;
        private readonly WorldModel virtualMap;

        public Renderer(FakeWorld world)
        {
            this.world = world;

            var sorted = this.world.Subjects.OrderBy(x => x.Coordinates.X).ToList();
            virtualMap = new WorldModel(
                sorted.Select(x => x.Coordinates.X).First() - padding,
                sorted.Select(x => x.Coordinates.X).Last() + padding,
                this.world.Ticker,
                this.world.Subjects);

        }
    }
}