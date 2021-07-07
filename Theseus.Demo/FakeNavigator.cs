using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Theseus.Core;

namespace Theseus.Demo
{
    public class FakeNavigator : INavigation, IUpdatable
    {
        private readonly int _stepPerSec = 1;
        private readonly IGPS gps;
        private readonly Coordinates _currentCoordinates;
        private int _directedSpeed;

        private bool _isMoving = false;
        private Coordinates _currentTargetCoords;

        private float _navigationError = 0.4F;

        public FakeNavigator(IGPS gps, Coordinates currentCoordinates, Ticker ticker)
        {
            this._currentCoordinates = currentCoordinates;
            this.gps = gps;
            ticker.Subscribe(this);
        }

        public bool IsMoving()
        {
            return _isMoving;
        }

        public void NavigateTo(Coordinates currentClientCoords)
        {
            _currentTargetCoords = currentClientCoords;
            var currentCoord = gps.GetGPSCoords().X;
            _directedSpeed = currentClientCoords.X - currentCoord > 0 ? +_stepPerSec : -_stepPerSec;
            _isMoving = true;
        }

        public void Update(long deltaT)
        {
            if (!_isMoving)
                return;

            float deltaDistance = deltaT * _directedSpeed / 1000;
            _currentCoordinates.X += deltaDistance;

            if (Math.Abs(_currentCoordinates.X - _currentTargetCoords.X) <= _navigationError)
            {
                _isMoving = false;
            }
        }
    }
}