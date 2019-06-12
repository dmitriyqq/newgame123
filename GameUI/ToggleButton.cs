using GameModel;
using Gwen.Control;

namespace GameUI
{
    public class ToggleButton : Button
    {
        private readonly Toggle _toggle;
        private readonly string _on;
        private readonly string _off ;
        
        public ToggleButton(Base parent, Toggle toggle, string on = "On", string off = "Off") : base(parent)
        {
            _toggle = toggle;
            toggle.OnToggle += Update;
            
            _on = on;
            _off = off;
            Update(toggle);
        }

        protected override void OnClicked(int x, int y)
        {
            base.OnClicked(x, y);
            _toggle.ToggleMe();
        }

        private void Update(bool toggle)
        {
            SetText(toggle ? _on : _off);
        }
    }
}