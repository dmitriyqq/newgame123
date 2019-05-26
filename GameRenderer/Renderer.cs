using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameModel;
using GameUI;
using GlmNet;
using ModelLoader;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GameRenderer
{
    public class Renderer : GameWindow, IGameLoop
    {
        private readonly Model _model;
        private UserInterface _ui;
        
        public Toggle IsPlaying { get; set; } = new Toggle();

        // Debug 
        private readonly Mesh _cameraDebug;
        private readonly Mesh _rayMesh;

        private readonly DrawablesFactory _factory;
        private List<Mesh> _pipeline = new List<Mesh>();
        private readonly List<IDrawable> _drawables = new List<IDrawable>();
        private DirectionalLight _dirLight;

        public Camera Camera { get; }
        public Logger Logger { get; private set; }
        
        public Renderer(Model model, AssetStore store) : base(
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
            Camera = new Camera();

            _cameraDebug = new Mesh(new AxiesGeometry(Camera.Target, Camera.Forward, Camera.Right, Camera.Up), new ColorMaterial());
            var gridGeometry = new Mesh(new GridGeometry(100.0f, 10.0f, new vec4(1.0f, 1.0f, 1.0f, 0.4f)), new ColorMaterial());
            var axisGeometry = new Mesh(new AxiesGeometry(new vec3(0.0f),new vec3(0.0f, 0.0f, 1.0f),new vec3(1.0f, 0.0f, 0.0f), new vec3(0.0f, 1.0f, 0.0f), 10.0f), new ColorMaterial());
            _rayMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.0f, 0.0f, 1.0f),
                new vec4(0.0f, 1.0f, 0.0f, 1.0f)), new ColorMaterial());

            var skyMaterial = new CubemapMaterial {Texture = new CubemapTexture(GetSkybox())};
            Mesh skyBox = new SkyBoxMesh(new CubeGeometry(), skyMaterial) {Scale = new vec3(100.0f, 100.0f, 100.0f)};

            _factory = new DrawablesFactory(store);
            
            _drawables.Add(skyBox);
            _drawables.Add(_cameraDebug);
            _drawables.Add(gridGeometry);
            _drawables.Add(axisGeometry);
            _drawables.Add(_rayMesh);
            
            foreach (var weaponType in model.WeaponTypes)
            {
                _drawables.Add(new BulletParticleEngine(weaponType.Bullets));
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
            _dirLight = new DirectionalLight
            {
                Direction = glm.normalize(new vec3(0.5f, 1.0f, 0.3f)),
                Ambient = new vec3(0.05f, 0.05f, 0.05f),
                Diffuse = new vec3(0.7f, 0.7f, 0.7f),
                Specular = new vec3(0.4f, 0.4f, 0.4f),
                Program = new LightMaterial().Program,
            };

            _dirLight.Uniform(new LightMaterial().Program);
        }

        public void AddUserInterface(UserInterface ui)
        {
            _ui = ui; 
            ui.AddRendererMenu(this);
        }

        private void ConstructPipeline()
        {
            _pipeline = new List<Mesh>();
            
            foreach (var drawable in _drawables)
            {
                var l = drawable.GetAllMeshes();
                foreach (var mesh in l)
                {
                    _pipeline.Add(mesh);
                }
            }
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

            IDrawable um;
            if (gameObject is Map map)
            {
                var mapMaterial = new LightMaterial
                {
                    diffuse = new Texture("textures/desert.jpeg"),
                    specular = new Texture("textures/desert.jpeg"),
                    shininess = 1.0f
                };

                um = new Mesh(new MapGeometry(map), mapMaterial);
            }
            else
            {
                um = _factory.CreateDrawableForGameObject(gameObject);
                um.Rotation = new vec3(1.0f, 0.0f, 0.0f);
            }
            
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

            foreach (var shader in GetAllShaders())
            {
                shader.UniformCamera(Camera);
            }

            foreach (var mesh in _pipeline)
            {
                mesh.Draw();
            }

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

        private IEnumerable<Material> GetAllMaterials()
        {
            return _pipeline.Select(m => m.Material);
        }

        private IEnumerable<ShaderProgram> GetAllShaders()
        {
            return GetAllMaterials().Select(m => m.Program).Distinct();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            _ui.MouseMove(e);
            Camera.OnMouseMove(e);
            var (start, dir) = Camera.CastRay(e.X, e.Y);
            (_rayMesh.Geometry as LineGeometry)?.Update(start.ToGlm(), (dir * 1000.0f).ToGlm());

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            _ui.MouseDown(e);
            Camera.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            _ui.MouseUp(e);
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

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            _ui.KeyUp(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            _ui.KeyDown(e);
        }

        public List<string> GetSkybox()
        {
            return new List<string>
            {
                "textures/skybox/front.jpg",
                "textures/skybox/back.jpg",
                "textures/skybox/top.jpg",
                "textures/skybox/bottom.jpg",
                "textures/skybox/right.jpg",
                "textures/skybox/left.jpg",
            };
        }

        
    }
}