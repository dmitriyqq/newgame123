using System;
using System.Linq;
using GameModel;
using GameModel.GameObjects;
using GameRenderer;
using GameRenderer.Metadata.Assets;
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
        private readonly Logger _logger;
        private readonly IRayCaster _rayCaster;
        private readonly Renderer _renderer;

        private CollapsibleList _listBox;
        private CollapsibleCategory _actionsCategory;
        private CollapsibleCategory _assetsCategory;
        private IDrawable _cursor;
        private Asset _selectedAsset;
        private Map _map;
        
        public Constructor(Base parent, Logger logger, AssetStore assetStore, UserInterface ui, IRayCaster rayCaster, Renderer renderer) : base(parent)
        {
            Dock = Pos.Fill;
            _assetStore = assetStore;
            _ui = ui;
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
            
            foreach (var asset in _assetStore.AssetsFile.Assets)
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

            if (_ui.Model.GameObjects.FirstOrDefault(o => o is Map) is Map map)
            {
                _map = map;
                _ui.Renderer.MouseMove += MoveAsset;
                _ui.Renderer.MouseDown += PlaceAsset;
            }
            else
            {
                _logger.Warning("No map for asset");
            }
        }

        private void MoveAsset(object s, MouseMoveEventArgs e)
        {
            var (start, dir) = _rayCaster.CastRay(e.X, e.Y);

            var v =_ui.Model.Engine.IntersectMap(_map, start, dir, out var normal);

            if (!v.HasValue)
            {
                return;
            }

            _cursor.Position = v.Value.ToGlm() + new vec3(0.0f, 5.0f, 0.0f);
            _cursor.Rotation = normal.ToGlm();
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
                    gameObject.Asset = _selectedAsset.Name;
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
                _ui.Renderer.MouseMove -= MoveAsset;
                _ui.Renderer.MouseDown -= PlaceAsset;
            
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