using GameRenderer.Metadata.Assets;
using Gwen;
using Gwen.Skin;
using Base = Gwen.Control.Base;

namespace GameUI.AssetViewers
{
    public abstract class AssetTypeWidget : Gwen.Control.Base
    {
        protected AssetTypeWidget(Base parent) : base(parent)
        {
            Dock = Pos.Fill;
        }

        public abstract Asset GetCreatedAsset(string gameObjectType = null);
    }
}