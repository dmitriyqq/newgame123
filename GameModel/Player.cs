using System.Numerics;

namespace GameModel
{
    public class Player
    {
        public string Name { get; set; }
        
        public Vector3 Color { get; set; }

        public Player(string name, Vector3 color)
        {
            Color = color;
            Name = name;
        }
    }
}