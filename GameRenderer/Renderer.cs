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
    public class Renderer : GameWindow
    {
        private Model model;

        private UserInterface ui;

        private Camera camera;

        private UnitMesh selectedMesh;

        // Debug 
        private Mesh cameraDebug;
        private Mesh gridGeometry;
        private Mesh axiesGeometry;
        private Mesh markerMesh;
        private Mesh rayMesh;

        private readonly CubeGeometry cube;

        private List<Mesh> pipeline = new List<Mesh>();

        private Scene unitModel;

        private Scene homeScene;

        private Scene turretScene;
        
        private List<IDrawable> drawables = new List<IDrawable>();

        public Renderer(Model model) : base(
            800,
            600,
            GraphicsMode.Default,
            "Game",
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4,
            5,
            GraphicsContextFlags.ForwardCompatible | GraphicsContextFlags.Debug)
        {
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

            drawables.Add(cameraDebug);
            drawables.Add(gridGeometry);
            drawables.Add(axiesGeometry);
            drawables.Add(markerMesh);
            drawables.Add(rayMesh);

            foreach (var weaponType in model.weaponTypes)
            {
                drawables.Add(new BulletParticleEngine(weaponType.Bullets));
            }
            
            model.OnAddUnit += registerMesh;
            model.OnRemoveUnit += deleteMesh;
            
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthTest); 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);  

            ui = new UserInterface(Width, Height, this);
            
            model.Start();
        }

        private void constructPipeline()
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

        private void deleteMesh(Unit unit)
        {
            foreach (var drawable in drawables)
            {
                if (drawable is UnitMesh um && um.Unit == unit)
                {
                    drawables.Remove(um);
                    break;
                }
            }

            constructPipeline();
        }

        private void registerMesh(Unit unit)
        {
            if (unitModel == null || homeScene == null || turretScene == null)
            {
                loadModels();
            }

            Console.WriteLine("new unit");

            Scene um;

            if (unit is Home)
            {
                um = homeScene.Clone();
                um.Scale = new vec3(0.8f, 0.8f, 0.8f);
            } else if (unit is Turret)
            {
                um = turretScene.Clone();
//                um.Scale = new vec3(0.01f, 0.01f, 0.01f);
            }
            else
            {
                um = unitModel.Clone();
                um.Scale = new vec3(0.1f, 0.1f, 0.1f);
            }
            
            var t = new UnitMesh(um, unit);
            drawables.Add(t);
            constructPipeline();
        }

        private void loadModels()
        {
            unitModel = new Scene("./models/viper", "viper.obj");
            turretScene = new Scene("./models/turret", "turret.obj");
            homeScene = new Scene("./models/home", "home.obj");
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
                if (!selectedMesh.Unit.SelectUnit(m.Unit))
                {
                    SelectMesh(m);
                }
            }
            else if(p.HasValue)
            {
                if (!selectedMesh.Unit.SelectPosition(p.Value.ToVector()))
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
                ui.CreateDebugWindow(selectedMesh.Unit);
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
                    var point = unitMesh.Unit.IsIntersect(start.ToVector(), dir.ToVector());
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
            
            model.Update(deltaTime);
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
            ui.MouseDown(e);
            camera.OnMouseDown(e);
            HandleMouseClick(e.Mouse.X, e.Mouse.Y);

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            ui.MouseUp(e);
            camera.OnMouseUp(e);
            base.OnMouseUp(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0,0,Width, Height);

            var _projectionMatrix = Matrix4.CreateTranslation(
                new Vector3(-Width / 2.0f, -Height / 2.0f, 0)) *
                Matrix4.CreateScale(new Vector3(1, -1, 1)) *
                Matrix4.CreateOrthographic(Width, Height, -1.0f, 1.0f);

            ui.Resize(_projectionMatrix, Width, Height);
            
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
    }
}