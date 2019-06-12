using System.Collections.Generic;
using GameModel.GameObjects;

namespace GameModel
{
    public class GameSave
    {
        public List<Player> Players { get; set; }
        public List<GameObject> GameObjects { get; set; }
        public float Time { get; set; }
        public int Ticks { get; set; }
    }
}