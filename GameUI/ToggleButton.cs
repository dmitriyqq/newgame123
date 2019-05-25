using GameModel;
using Gwen.Control;

namespace GameUI
{
    public class ToggleButton : Button
    {
        private Toggle toggle;

        private string on;

        private string off ;
        
        public ToggleButton(Base parent, Toggle toggle, string on = "On", string off = "Off") : base(parent)
        {
            this.toggle = toggle;
            toggle.OnToggle += Update;
            
            this.on = on;
            this.off = off;
            Update(toggle);
        }

        protected override void OnClicked(int x, int y)
        {
            base.OnClicked(x, y);
            toggle.ToggleMe();
        }

        private void Update(bool toggle)
        {
            if (toggle)
            {
                SetText(on);
            }
            else
            {
                SetText(off);
            }
        }
    }
}