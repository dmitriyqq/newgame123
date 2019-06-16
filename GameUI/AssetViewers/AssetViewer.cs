using System;
using GameRenderer.Metadata.Assets;
using Gwen;
using Gwen.Control;

namespace GameUI.AssetViewers
{
    public class AssetViewer : WindowControl
    {
        private readonly UserInterface _ui;
        private readonly string _inputtedAssetName;
        private readonly string _assetWidgetType;
        
        private Asset _asset;
        private VerticalSplitter _rootSplitter;
        private VerticalSplitter _controlsSplitter;
        private AssetTypeWidget _assetWidget;
        
        public AssetViewer(Base parent, UserInterface ui, string inputtedAssetName, string assetWidgetType) : base(parent)
        {
            _inputtedAssetName = inputtedAssetName;
            _assetWidgetType = assetWidgetType;
            _ui = ui;

            Title = "Asset Viewer";
            
            CreateLayout();
        }

        private AssetTypeWidget CreateAssetWidget()
        {
            switch (_assetWidgetType)
            {
                case "Static": return new StaticAssetWidget(_controlsSplitter, _ui.Logger) {Dock = Pos.Fill};
                case "Animated": return new AnimatedAssetWidget(_controlsSplitter, _ui.Logger) {Dock = Pos.Fill};
                default:
                    throw new NotImplementedException("Asset is not supported by viewer");
            }
        }

        public override void Think()
        {
            base.Think();
        }

        private void CreateLayout()
        {
            SetSize(700, 500);
            _rootSplitter = new VerticalSplitter(this) {Dock = Pos.Fill, SplittersVisible = true};
            _controlsSplitter = new VerticalSplitter(_rootSplitter) {Dock = Pos.Fill, SplittersVisible = true};

            var gameObjectWidget = new GameObjectWidget(_controlsSplitter, _ui.Model.GameObjectTypes) {Dock = Pos.Left};

            _assetWidget = CreateAssetWidget();
            
            _controlsSplitter.SetPanel(0, gameObjectWidget);
            _controlsSplitter.SetPanel(1, _assetWidget);

            var rendererImage = new ImagePanel(_rootSplitter) {Dock = Pos.Fill, ImageName ="./textures/fa-image.png"};
            
            _rootSplitter.SetPanel(0, _controlsSplitter);
            _rootSplitter.SetPanel(1, rendererImage);
            
            _ui.SetCenter(this);
        }

        private void RenderAsset()
        {
            
        }
    }
}