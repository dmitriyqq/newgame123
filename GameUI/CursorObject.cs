
using System.Numerics;
using ModelLoader;

namespace GameUI
{
    public class CursorObject
    {
        public Asset Asset { get; set; }
        
        public bool Active { get; set; }
        
        public Vector3 Position { get; set; }
    }
}