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

        private List<Mesh> rootMeshes = new List<Mesh>();

        private Camera camera;

        private Mesh cameraDebug;

        private Mesh gridGeometry;

        private Mesh axiesGeometry;

        private Mesh markerMesh;

        private UnitMesh selectedMesh;

        private readonly CubeGeometry cube;

        private Mesh rayMesh;

        private ParticleEngine particlesEngine;

        private UserInterface ui;

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

            particlesEngine = new ParticleEngine();
            cameraDebug = new Mesh(new AxiesGeometry(camera.Target, camera.Forward, camera.Right, camera.Up), new ColorMaterial());
            gridGeometry = new Mesh(new GridGeometry(100.0f, 1.0f, new vec4(1.0f, 1.0f, 1.0f, 0.4f)), new ColorMaterial());
            axiesGeometry = new Mesh(new AxiesGeometry(
                new vec3(0.0f), 
                new vec3(0.0f, 0.0f, 1.0f),
                new vec3(1.0f, 0.0f, 0.0f),
                new vec3(0.0f, 1.0f, 0.0f), 10.0f), new ColorMaterial());
            markerMesh = new Mesh(cube, new TextureMaterial());
            rayMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.0f, 0.0f, 1.0f),
                new vec4(0.0f, 1.0f, 0.0f, 1.0f)), new ColorMaterial());

            rootMeshes.Add(cameraDebug);
            rootMeshes.Add(gridGeometry);
            rootMeshes.Add(axiesGeometry);
            rootMeshes.Add(markerMesh);
            rootMeshes.Add(rayMesh);

            model.OnAddUnit += registerMesh;
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.Blend); 
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);  
            
            var tmp = new Scene("models/viper.obj");
            ui = new UserInterface(Width, Height, this);
            ui.Width = Width;
            ui.Height = Height;
        }

        private void registerMesh(Unit unit)
        {
            Console.WriteLine("new unit");
            var t = new UnitMesh(cube, new TextureMaterial(), unit);
            rootMeshes.Add(t);
        }

        private void HandleMouseClick(float x, float y)
        {
            Console.WriteLine($"Mouse click at x: {x} y:{y}");
            var ray = camera.CastRay(x, y);

            var m = intersectMeshes(ray.start, ray.dir);
            var p = intersectPlane(ray.start, ray.dir);

            if (selectedMesh == null)
            {
                SelectMesh(m);
            }
            else
            {
                if (m != null)
                {
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
            foreach (var mesh in rootMeshes)
            {
                if (mesh is UnitMesh unitMesh)
                {
                    var point = unitMesh.Unit.IsIntersect(start.ToVector(), dir.ToVector());
                    if (point != null)
                    {
                        Console.WriteLine($"Mesh selected");
                        markerMesh.Position = point.ToGlm();
                        return unitMesh;
                    }
                }
            }

            return null;
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            UpdateCamera((float) e.Time);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var shader in GetAllShaders())
            {
                shader.UniformCamera(camera);
            }
            
            foreach (var mesh in rootMeshes)
            {
                mesh.Draw();
            }
            
            particlesEngine.Draw();
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

            model.Update((float) e.Time);
            Weapon.Update((float) e.Time);
            ui.Update((float)e.Time);
            UpdateParticleEngine();
        }
        
        public IEnumerable<Material> GetAllMaterials()
        {
            return rootMeshes.Select(m => m.Material);
        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            return GetAllMaterials().Select(m => m.Program).Distinct();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            camera.OnMouseMove(e);
            var ray = camera.CastRay(e.X, e.Y);
            (rayMesh.Geometry as LineGeometry).Update(ray.start, ray.dir * 1000.0f);
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            camera.OnMouseDown(e);
            
            HandleMouseClick(e.Mouse.X, e.Mouse.Y);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            camera.OnMouseUp(e);
            base.OnMouseUp(e);
        }

        private void UpdateParticleEngine()
        {
            var list = Weapon.bullets;

            var positions = new float[4 * list.Count];
            var colors = new float[4 * list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                var index = i * 4;
                positions[index + 0] = list[i].Position.X;
                positions[index + 1] = list[i].Position.Y;
                positions[index + 2] = list[i].Position.Z;
                positions[index + 3] = 1.0f;
                
                colors[index + 0] = list[i].Color.X;
                colors[index + 1] = list[i].Color.Y;
                colors[index + 2] = list[i].Color.Z;
                colors[index + 3] = 1.0f;
            }
            
            ParticleEngine.program.UniformCamera(camera);
            ParticleEngine.program.UniformVec3("cameraUp", camera.Up);
            ParticleEngine.program.UniformVec3("cameraRight", camera.Right);
            
            particlesEngine.Update(positions, colors, list.Count);
        }
        
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0,0,Width, Height);

            var _projectionMatrix = Matrix4.CreateTranslation(new Vector3(-Width / 2.0f, -Height / 2.0f, 0)) * Matrix4.CreateScale(new Vector3(1, -1, 1)) * Matrix4.CreateOrthographic(Width, Height, -1.0f, 1.0f);

            ui.Resize(_projectionMatrix, Width, Height);
            
            base.OnResize(e);
        }
    }
}