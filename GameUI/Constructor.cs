using System;
using System.Numerics;
using GameModel;
using GameModel.GameObjects;
using GameRenderer;
using GameUtils;
using GlmNet;
using Gwen;
using Gwen.Control;
using ModelLoader;
using OpenTK.Input;

namespace GameUI
{
    public class Constructor : Base
    {
        private readonly AssetStore _assetStore;
        private readonly UserInterface _ui;
        private readonly Map _map;
        private readonly Logger _logger;
        private readonly IRayCaster _rayCaster;
        private readonly IRenderer _renderer;
        private CollapsibleList _listBox;
        private CollapsibleCategory _actionsCategory;
        private CollapsibleCategory _assetsCategory;
        private IDrawable _cursor;
        private Asset _selectedAsset;
        
        public Constructor(Base parent, Logger logger, AssetStore assetStore, UserInterface ui, Map map, IRayCaster rayCaster, IRenderer renderer) : base(parent)
        {
            Dock = Pos.Fill;
            _assetStore = assetStore;
            _ui = ui;
            _map = map;
            _rayCaster = rayCaster;
            _logger = logger;
            _renderer = renderer;

            _assetStore.OnAssetUpdate += CreateListBox;
            CreateListBox();
        }

        private void CreateListBox()
        {
            _listBox = new CollapsibleList(this) {Dock = Pos.Fill};

            _actionsCategory = _listBox.Add("Actions");
            _assetsCategory = _listBox.Add("Assets");
            _assetsCategory.Selected += HandleAssetSelected;

            var createButton = _actionsCategory.Add("Create");
            createButton.Clicked += HandleCreate;
            
            var deleteButton = _actionsCategory.Add("Delete");
            deleteButton.Clicked += HandleDelete;

            var editButton = _actionsCategory.Add("Edit");
            editButton.Clicked += HandleEdit;
            
            foreach (var asset in _assetStore.Assets)
            {
                CreateRow(asset);
            }

        }

        private void HandleDelete(Base control, EventArgs e)
        {
            Unselect();
        }
        
        private void HandleEdit(Base control, EventArgs e)
        {
            Unselect();
        }
        
        private void HandleCreate(Base control, EventArgs e)
        {
            _ui.Canvas.AddChild(new CreateAssetModal(_ui.Canvas, _assetStore));
            Unselect();
        }

        private void HandleAssetSelected(Base control, ItemSelectedEventArgs args)
        {
            if (!(args.SelectedItem.UserData is Asset asset)) return;

            _selectedAsset = asset;
            _cursor = _renderer.AddDrawable(asset);

            _ui.Window.MouseMove += MoveAsset;
            _ui.Window.MouseDown += PlaceAsset;
        }

        private void MoveAsset(object s, MouseMoveEventArgs e)
        {
            var (start, dir) = _rayCaster.CastRay(e.X, e.Y);
            var v =_ui.Model.Engine.IntersectMap(_map, start, dir, out var normal);

            if (v.HasValue)
            {
                _cursor.Position = v.Value.ToGlm() + new vec3(0.0f, 5.0f, 0.0f);
                _cursor.Rotation = normal.ToGlm();
            }
        }
        
        private void PlaceAsset(object s, MouseButtonEventArgs e)
        {
            _logger.Info($"Placing object");

            try
            {
                var typeName = _selectedAsset.GameObjectType;
                var gameObject = ReflectionHelper.CreateObjectFromType<GameObject>(typeName);

                if (gameObject != null)
                {
                    gameObject.Asset = _selectedAsset;
                    gameObject.Transform.Position = _cursor.Position.ToVector3();

                    _ui.Model.AddGameObject(gameObject);
                }
                else
                {
                    _logger.Error($"Couldn't create gameObject of type {typeName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _ui.Window.MouseMove -= MoveAsset;
                _ui.Window.MouseDown -= PlaceAsset;
            
                Unselect();    
            }
        }

        private void CreateRow(Asset asset)
        {
            var row = _assetsCategory.Add(asset?.Name ?? "No name");
            row.UserData = asset;
            
        }

        private void Unselect()
        {
            _renderer.RemoveDrawable(_cursor);

            _cursor = null;
            _selectedAsset = null;
            
            _listBox.UnselectAll();
        }
    }
}