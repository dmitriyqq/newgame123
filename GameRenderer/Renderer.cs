using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GameModel;
using GameUI;
using GlmNet;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace GameRenderer
{
    public class Renderer : GameWindow, IGameLoop
    {
        private Model model;

        private UserInterface ui;

        private Camera camera;

        private UnitMesh selectedMesh;
        
        public Toggle IsPlaying { get; set; } = new Toggle();

        // Debug 
        private Mesh cameraDebug;
        private Mesh gridGeometry;
        private Mesh axiesGeometry;
        private Mesh markerMesh;
        private Mesh rayMesh;
        private Mesh sphere;

        private readonly CubeGeometry cube;

        private List<Mesh> pipeline = new List<Mesh>();

        private Dictionary<string, Scene> scenes = new Dictionary<string, Scene>();
        
        private List<IDrawable> drawables = new List<IDrawable>();

        private Mesh skyBox;

        private DirectionalLight dirLight;

        public Logger Logger { get; private set; }
        
        public Renderer(Model model) : base(
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
            
            
            this.model = model;
            camera = new Camera();
            cube = new CubeGeometry();

            cameraDebug = new Mesh(new AxiesGeometry(camera.Target, camera.Forward, camera.Right, camera.Up), new ColorMaterial());
            gridGeometry = new Mesh(new GridGeometry(100.0f, 10.0f, new vec4(1.0f, 1.0f, 1.0f, 0.4f)), new ColorMaterial());
            axiesGeometry = new Mesh(new AxiesGeometry(
                new vec3(0.0f), 
                new vec3(0.0f, 0.0f, 1.0f),
                new vec3(1.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f), 10.0f), new ColorMaterial());
            markerMesh = new Mesh(cube, new TextureMaterial());
            rayMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.0f, 0.0f, 1.0f),
                new vec4(0.0f, 1.0f, 0.0f, 1.0f)), new ColorMaterial());

            var skyMaterial = new CubemapMaterial();
            skyMaterial.Texture = new CubemapTexture(GetSkybox());
            skyBox = new SkyBoxMesh(new CubeGeometry(), skyMaterial);
            skyBox.Scale = new vec3(100.0f, 100.0f, 100.0f);

            drawables.Add(skyBox);
            drawables.Add(cameraDebug);
            drawables.Add(gridGeometry);
            drawables.Add(axiesGeometry);
            drawables.Add(markerMesh);
            drawables.Add(rayMesh);
            
            var partEngine = new TexturedParticleEngine();
            ((TexturedParticlesMaterial) partEngine.Material).Texture = new Texture("textures/particle.jpg");

            float[] pos = new float[500 * 4];
            float[] col = new float[500 * 4];

            for (int i = 0; i < 500; i++)
            {
                var p = Vector3Helper.Random() * 5.0f;
                var c = Vector3Helper.Random() * 5.0f;

                var index = i * 4;

                pos[index + 0] = p.X;
                pos[index + 1] = p.Y;
                pos[index + 2] = p.Z;
                pos[index + 3] = 1.0f;
                
                col[index + 0] = p.X;
                col[index + 1] = p.Y;
                col[index + 2] = p.Z;
                col[index + 3] = 0.8f;
            }
            
            partEngine.Update2(pos, col, 500);
            drawables.Add(partEngine);
            
            foreach (var weaponType in model.WeaponTypes)
            {
                drawables.Add(new BulletParticleEngine(weaponType.Bullets));
            }
            
            model.OnAddGameObject += registerMesh;
            model.OnRemoveGameObject += DeleteMesh;
            
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest); 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            CreateLights();
            LoadModels();

            model.Start();
        }

        private void CreateLights()
        {
            dirLight = new DirectionalLight
            {
                Direction = glm.normalize(new vec3(0.5f, 1.0f, 0.3f)),
                Ambient = new vec3(0.2f, 0.2f, 0.2f),
                Diffuse = new vec3(0.8f, 0.8f, 0.8f),
                Specular = new vec3(0.5f, 0.5f, 0.5f),
                Program = new LightMaterial().Program,
            };

            dirLight.Uniform(new LightMaterial().Program);
        }

        public void AddUserInterface(UserInterface ui)
        {
            this.ui = ui; 
            ui.AddMenu(model, this);
        }

        private void ConstructPipeline()
        {
            pipeline = new List<Mesh>();
            
            foreach (var drawable in drawables)
            {
                var l = drawable.GetAllMeshes();
                foreach (var mesh in l)
                {
                    pipeline.Add(mesh);
                }
                
            }
        }

        private void DeleteMesh(GameObject gameObject)
        {
            foreach (var drawable in drawables)
            {
                if (drawable is UnitMesh um && um.GameObject == gameObject)
                {
                    drawables.Remove(um);
                    break;
                }
            }

            ConstructPipeline();
        }

        private void registerMesh(GameObject gameObject)
        {
            Logger.Info("Registering Mesh");

            IDrawable um;

//            if (gameObject is Home)
//            {
//                um = scenes["home"].Clone();
//                um.Scale = new vec3(0.8f, 0.8f, 0.8f);
//            } else if (gameObject is Turret)
//            {
//                um = scenes["turret"].Clone();
//            } else if (gameObject is Tank)
//            {
//                um = scenes["tank"].Clone();
//            } else if (gameObject is Buggy)
//            {
//                um = scenes["buggy"].Clone();
//            } else if(gameObject is Soldier)
//            {
//                um = scenes["soldier"].Clone();
//                um.Scale = new vec3(0.1f, 0.1f, 0.1f);
//            }
             if (gameObject is Map map)
            {
                var mapMaterial = new LightMaterial();

                mapMaterial.diffuse = new Texture("textures/desert.jpeg");
                mapMaterial.specular = new Texture("textures/desert.jpeg");
                mapMaterial.shininess = 1.0f;
                
                um = new Mesh(new MapGeometry(map), mapMaterial);
            }
            else
             {
                um = sphere.Clone(); //scenes["unit"].Clone();
                um.Rotation = new vec3(1.0f, 0.0f, 0.0f);
            }
            
            var t = new UnitMesh(um, gameObject);
            drawables.Add(t);
            ConstructPipeline();
        }

        private void LoadModels()
        {
            scenes["unit"] = new Scene("./models/trident", "trident3.obj");
            scenes["turret"] = new Scene("./models/turret", "turret.obj");
            scenes["home"] = new Scene("./models/home", "home.obj");
            // scenes["buggy"] = new Scene("./models/lexus", "lexus.obj");
            scenes["tank"] = new Scene("./models/tank", "tank.obj");
            sphere = new Mesh(new SphereGeometry(1.1f, new vec4(1.0f, 0.0f, 0.0f, 1.0f)), new ColorMaterial());
        }

        private void HandleMouseClick(float x, float y)
        {
            Console.WriteLine($"Mouse click at x: {x} y:{y}");
            var (start, dir) = camera.CastRay(x, y);
            
            var m = intersectMeshes(start, dir);
            var p = intersectPlane(start, dir);

            if (selectedMesh == null)
            {
                SelectMesh(m);
                return;
            }

            if (m != null)
            {
                camera.Follow(m);
                if (!selectedMesh.GameObject.SelectUnit(m.GameObject))
                {
                    SelectMesh(m);
                }
            }
            else if(p.HasValue)
            {
                if (!selectedMesh.GameObject.SelectPosition(p.Value.ToVector3()))
                {
                    selectedMesh.Selected = false;
                    selectedMesh = null;
                }
            }
        }

        private void SelectMesh(UnitMesh m)
        {
            if (selectedMesh != null)
            {
                selectedMesh.Selected = false;
            }

            selectedMesh = m;
            if (selectedMesh != null)
            {
                selectedMesh.Selected = true;
                ui.CreateDebugWindow(selectedMesh);
            }
        }
        
        private vec3? intersectPlane(vec3 start, vec3 dir)
        {
            // assuming vectors are all normalized
            var n = new vec3(0.0f, -1.0f, 0.0f);
            var p0 = new vec3(0.0f, 0.0f, 0.0f);
            
            float denom = glm.dot(n, dir); 
            if (denom > 1e-6) { 
                vec3 p0l0 = p0 - start; 
                float t = glm.dot(p0l0, n) / denom;

                var p = start + t * dir;
//                markerMesh.Position = p;
                return p; 
            } 
 
            return null; 
        }

        private UnitMesh intersectMeshes(vec3 start, vec3 dir)
        {
            foreach (var mesh in drawables)
            {
                if (mesh is UnitMesh unitMesh)
                {
                    var point = unitMesh.GameObject.IsIntersect(start.ToVector3(), dir.ToVector3());
                    if (point != null)
                    {
                        Console.WriteLine($"Mesh selected");
                        return unitMesh;
                    }
                }
            }

            return null;
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            float deltaTime = (float) e.Time;

            UpdateCamera(deltaTime);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                foreach (var drawable in drawables)
            {
                drawable.Update(deltaTime);
            }

            foreach (var shader in GetAllShaders())
            {
                shader.UniformCamera(camera);
            }

            foreach (var mesh in pipeline)
            {
                mesh.Draw();
            }

            ui.Draw();
            SwapBuffers();
        }

        private void UpdateCamera(float deltaTime)
        {
            camera.Width = Width;
            camera.Height = Height;

            if (cameraDebug.Geometry is AxiesGeometry g)
            {
                g.Update( camera.Target, camera.Forward, camera.Right, camera.Up);
            }

            cameraDebug.Position = new vec3(0.0f);
            cameraDebug.Rotation = new vec3(0.0f, 0.0f, -1.0f);
 
            camera.Move(Keyboard.GetState(), deltaTime);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            var deltaTime = (float) e.Time;

            if (IsPlaying)
            {
                model.Update(deltaTime);
            }

            ui.Update(deltaTime);
        }
        
        public IEnumerable<Material> GetAllMaterials()
        {
            return pipeline.Select(m => m.Material);
        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            return GetAllMaterials().Select(m => m.Program).Distinct();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            ui.MouseMove(e);
            camera.OnMouseMove(e);
            var ray = camera.CastRay(e.X, e.Y);
            (rayMesh.Geometry as LineGeometry)?.Update(ray.start, ray.dir * 1000.0f);

            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            ui.MouseDown(e);
            camera.OnMouseDown(e);

        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            ui.MouseUp(e);
            camera.OnMouseUp(e);

            HandleMouseClick(e.Mouse.X, e.Mouse.Y);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0,0,Width, Height);

            var _projectionMatrix = Matrix4.CreateTranslation(
                new Vector3(-Width / 2.0f, -Height / 2.0f, 0)) *
                Matrix4.CreateScale(new Vector3(1, -1, 1)) *
                Matrix4.CreateOrthographic(Width, Height, -1.0f, 1.0f);

            ui?.Resize(_projectionMatrix, Width, Height);
            
            base.OnResize(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
            ui.KeyUp(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            ui.KeyDown(e);
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