using System.Numerics;

namespace GameModel
{
    public class Player
    {
        public string Name { get; set; }
        
        public Vector Color { get; set; }

        public Player(string name, Vector color)
        {
            Color = color;
            Name = name;
        }
    }
}