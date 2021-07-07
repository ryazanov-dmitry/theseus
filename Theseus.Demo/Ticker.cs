using System;
using System.Collections.Generic;
using System.Timers;

namespace Theseus.Demo
{
    // Responsible for triggering all world parts for update.
    public class Ticker
    {
        private const int deltaT = 1000;
        private readonly List<IUpdatable> updatableList = new List<IUpdatable>();

        public Ticker()
        {
            var timer = new Timer();
            timer.Interval = deltaT;
            timer.Elapsed += Update;
            timer.Start();
        }

        public void Subscribe(IUpdatable updatable)
        {
            updatableList.Add(updatable);
        }

        private void Update(object sender, ElapsedEventArgs e)
        {
            updatableList.ForEach(x => x.Update(deltaT));
        }
    }
}