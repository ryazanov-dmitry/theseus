using System;

namespace Theseus.Core
{
    public static class Geometry
    {
        public static float Distance(float x1, float x2)
        {
            return Math.Abs(x1 - x2);
        }
    }
}