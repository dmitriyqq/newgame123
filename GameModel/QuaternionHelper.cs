using System;
using BepuUtilities;

namespace GameModel
{
    public static class QuaternionHelper
    {
        public static Quaternion Random()
        {
            var rand = new Random();
            return new Quaternion(
                (float) rand.NextDouble(),
                (float) rand.NextDouble(),
                (float) rand.NextDouble(),
                (float) rand.NextDouble()
            );
        }
    }
}