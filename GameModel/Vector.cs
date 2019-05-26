using System;
using System.Numerics;

namespace GameModel
{
    public class Vector3Helper
    {
        public static Vector3 Random()
        {
            var rand = new Random();
            
            return new Vector3((float) rand.NextDouble(), (float) rand.NextDouble(), (float) rand.NextDouble());
        }
    }
}