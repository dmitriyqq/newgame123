using System;
using GameModel;
using GameRenderer.Metadata.Assets;
using Gwen;
using Gwen.Control;
using ModelLoader;

namespace GameUI.AssetViewers
{
    public class AnimatedAssetWidget : AssetTypeWidget
    {
        private ListBox _animations;
        
        private readonly AnimatedModelAsset _asset;
        private readonly Logger _logger;
        private TextBox _textBox;
        private Button _openButton;
        public AnimatedAssetWidget(Base parent, Logger logger, AnimatedModelAsset asset = null) : base(parent)
        {
            _logger = logger;
            _animations = new ListBox(this) { Dock = Pos.Fill};
            
            if (asset == null)
            {
                _asset = new AnimatedModelAsset();
            }
        }

        private void CreateInputBox()
        {
            _textBox = new TextBox(this);

            _openButton = new Button(this) { Text = "Open"};
            _openButton.Clicked += HandleOpenModel;
        } 

        public override Asset GetCreatedAsset(string gameObjectType)
        {
            _asset.GameObjectType = gameObjectType;
            return _asset;
        }

        private void HandleOpenModel(Base control, EventArgs eventArgs)
        {
            try
            {
                
            }
            catch (Exception e)
            {
                
            }
        } 
    }
}