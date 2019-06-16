using System;
using System.Threading;
using System.Windows.Forms;
using GameModel;
using GameRenderer.Metadata.Assets;
using Gwen;
using Gwen.Control;
using Button = Gwen.Control.Button;
using TextBox = Gwen.Control.TextBox;

namespace GameUI.AssetViewers
{
    public class StaticAssetWidget : AssetTypeWidget
    {
        private readonly StaticModelAsset _asset;
        private readonly Button _loadModelButton;
        private readonly TextBox _label;
        private readonly Logger _logger;
        private Thread _thread;
        public StaticAssetWidget(Base parent, Logger logger, StaticModelAsset asset = null) : base(parent)
        {
            _logger = logger;
            _asset = asset ?? new StaticModelAsset();

            _label = new TextBox(this) {Dock = Pos.Top, Text = asset?.Scene ?? "Not set", KeyboardInputEnabled = false};
            
            _loadModelButton = new Button(this) { Text = "Load model", Dock = Pos.Top};
            _loadModelButton.Clicked += HandleLoadModelButtonClicked;
        } 

        public override Asset GetCreatedAsset(string gameObjectType)
        {
            _asset.GameObjectType = gameObjectType;
            return _asset;
        }

        private void HandleLoadModelButtonClicked(Base control, EventArgs eventArgs)
        {
            OpenFileDialog();
        }

        private void OpenFileDialog()
        {
            new Thread(() =>
            {
                try
                {
                    using (var dialog = new OpenFileDialog())
                    {
                        _logger.Info($"Open dialog to select scene file");
                        var status = dialog.ShowDialog();
                        _logger.Info($"Selected {dialog.SafeFileName} with status {status}");
                        _label.Text = dialog.SafeFileName;                            
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e);
                }

            }).Start();
        }
    }
}