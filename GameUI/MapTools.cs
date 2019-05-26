using GameModel;
using Gwen.Control;

namespace GameUI
{
    public class MapTools : Base
    {
        private Map _map;
        
        public MapTools(Base parent, Map map) : base(parent)
        {
            _map = map;
        }
    }
}