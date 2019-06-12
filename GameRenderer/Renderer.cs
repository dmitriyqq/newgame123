using System;
using System.Collections.Generic;
using GameModel;
using GameModel.GameObjects;
using GameRenderer.Materials;
using GameRenderer.Metadata.Assets;
using GameRenderer.OpenGL;
using GlmNet;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GameRenderer
{
    public class Renderer : GameWindow, IGameLoop
    {
        private readonly Model _model;
        private IUserInterface _ui;
        public Toggle IsPlaying { get; set; } = new Toggle();

        // TODO remove debug
        private readonly Mesh _cameraDebug;
        private readonly Mesh _rayMesh;

        private readonly DrawablesFactory _factory;
        private readonly MaterialManager _materialManager;
        private readonly AssetsManager _assetsManager;
        private readonly List<IDrawable> _drawables = new List<IDrawable>();
        private readonly List<Light> _lights = new List<Light>();

        private Pipeline _pipeline;
        public Camera Camera { get; }
        public Logger Logger { get; }
        public List<Asset> Assets { get; }
        
        public Renderer(Model model, AssetsFile assetsFile) : base(
            1920,
            1000,
            GraphicsMode.Default,
            "Game",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4,
            5,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
            Logger = new Logger("Render");
            Logger.Info("Created renderer");
            
            
            _model = model;
            _materialManager = new MaterialManager(assetsFile.Materials, Logger);
            _assetsManager = new AssetsManager(assetsFile.Assets, Logger);
            Assets = assetsFile.Assets;
            Camera = new Camera();

            _cameraDebug = new Mesh(new AxiesGeometry(Camera.Target, Camera.Forward, Camera.Right, Camera.Up), _materialManager.GetMaterial(Constants.DebugMaterial));
            var gridGeometry = new Mesh(new GridGeometry(100.0f, 10.0f, new vec4(1.0f, 1.0f, 1.0f, 0.4f)), _materialManager.GetMaterial(Constants.DebugMaterial));
            var axisGeometry = new Mesh(new AxiesGeometry(new vec3(0.0f),new vec3(0.0f, 0.0f, 1.0f),new vec3(1.0f, 0.0f, 0.0f), new vec3(0.0f, 1.0f, 0.0f), 10.0f), _materialManager.GetMaterial(Constants.DebugMaterial));
            _rayMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.0f, 0.0f, 1.0f),
                new vec4(0.0f, 1.0f, 0.0f, 1.0f)), _materialManager.GetMaterial(Constants.DebugMaterial));

            _factory = new DrawablesFactory(this, _materialManager, _assetsManager,Logger);
            
            _drawables.Add(_cameraDebug);
            _drawables.Add(gridGeometry);
            _drawables.Add(axisGeometry);
            _drawables.Add(_rayMesh);

            
            foreach (var weaponType in model.WeaponTypes)
            {
                // _drawables.Add(new BulletParticleEngine(weaponType.Bullets, _materialManager.GetMaterial("temp")));
            }
            
            model.OnAddGameObject += RegisterMesh;
            model.OnRemoveGameObject += DeleteMesh;
            
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest); 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            CreateLights();

            model.Start();
        }

        private void CreateLights()
        {
            _lights.Add(new DirectionalLight
            {
                Direction = glm.normalize(new vec3(0.0f, -1.0f, 0.0f)),
                Ambient = new vec3(0.05f, 0.05f, 0.05f),
                Diffuse = new vec3(0.7f, 0.7f, 0.7f),
                Specular = new vec3(0.4f, 0.4f, 0.4f)
            });

            foreach (var light in _lights)
            {
                foreach (var material in _materialManager.Materials)
                {
                    light.Uniform(material.Program);
                }
            }
        }

        public void AddUserInterface(IUserInterface ui)
        {
            _ui = ui;
        }

        public IDrawable AddDrawable(Asset asset)
        {
            var drawable = _factory.CreateDrawableForAsset(asset, null);
            _drawables.Add(drawable);
            ConstructPipeline();
            return drawable;
        }
        
        public void RemoveDrawable(IDrawable drawable)
        {
            _drawables.Remove(drawable);
            ConstructPipeline();
        }
        
        private void ConstructPipeline()
        {
            _pipeline = new Pipeline(_drawables);
        }

        private void DeleteMesh(GameObject gameObject)
        {
            foreach (var drawable in _drawables)
            {
                if (drawable is UnitMesh um && um.GameObject == gameObject)
                {
                    _drawables.Remove(um);
                    break;
                }
            }

            ConstructPipeline();
        }

        private void RegisterMesh(GameObject gameObject)
        {
            Logger.Info("Registering Mesh");

            var um = _factory.CreateDrawableForGameObject(gameObject);
            if (um == null)
            {
                Logger.Info("Can not create drawable for game object see errors before");
                return;
            }

            um.Rotation = new vec3(1.0f, 0.0f, 0.0f);
            var t = new UnitMesh(um, gameObject);
            _drawables.Add(t);
            ConstructPipeline();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            var deltaTime = (float) e.Time;

            UpdateCamera(deltaTime);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                foreach (var drawable in _drawables)
            {
                drawable.Update(deltaTime);
            }

            foreach (var material in _materialManager.Materials)
            {
                material.Program.UniformCamera(Camera);
            }

            _pipeline.Draw();

            _ui.Draw();
            SwapBuffers();
        }

        private void UpdateCamera(float deltaTime)
        {
            Camera.Width = Width;
            Camera.Height = Height;

            if (_cameraDebug.Geometry is AxiesGeometry g)
            {
                g.Update( Camera.Target, Camera.Forward, Camera.Right, Camera.Up);
            }

            _cameraDebug.Position = new vec3(0.0f);
            _cameraDebug.Rotation = new vec3(0.0f, 0.0f, -1.0f);
 
            Camera.Move(Keyboard.GetState(), deltaTime);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var deltaTime = (float) e.Time;

            if (IsPlaying)
            {
                _model.Update(deltaTime);
            }

            _ui.Update(deltaTime);
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Camera.OnMouseMove(e);

            var (start, dir) = Camera.CastRay(e.X, e.Y);
            (_rayMesh.Geometry as LineGeometry)?.Update(start.ToGlm(), (dir * 1000.0f).ToGlm());

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            Camera.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Camera.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0,0,Width, Height);

            var projectionMatrix = Matrix4.CreateTranslation(
                new Vector3(-Width / 2.0f, -Height / 2.0f, 0)) *
                Matrix4.CreateScale(new Vector3(1, -1, 1)) *
                Matrix4.CreateOrthographic(Width, Height, -1.0f, 1.0f);

            _ui?.Resize(projectionMatrix, Width, Height);
            
            base.OnResize(e);
        }
        
    }
}
