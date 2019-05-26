using System;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using GameModel.Tasks;
using GlmNet;
using OpenTK;

namespace GameRenderer
{
    public class     UnitMesh : IDrawable
    {
        public GameObject GameObject;

        private IDrawable Drawable;
        
        public IDrawable Parent { get; set; }

        private bool selected = true;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                foreach (var child in childs)
                {
                    child.Visible = value;
                }
            }
        }

        private Mesh TargetLineMesh;

        private Mesh HitBoxMesh;

        private Mesh OrientationMesh;

        private List<Mesh> childs;
        
        public vec4 color => GameObject?.Player == null ? new vec4(1.0f) : new vec4(GameObject.Player.Color.ToGlm(), 0.3f); 

        public UnitMesh(IDrawable drawable, GameObject gameObject)
        {
            Drawable = drawable;
            GameObject = gameObject;

            HitBoxMesh = new Mesh(new SphereGeometry(GameObject.Radius, color),
                new ColorMaterial());
            
            TargetLineMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 1.0f, 0.4f, 1.0f)), new ColorMaterial());

            OrientationMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.9f, 0.9f, 1.0f)), new ColorMaterial());
            
            childs = new List<Mesh>();
            childs.Add(TargetLineMesh);
            childs.Add(HitBoxMesh);
            childs.Add(OrientationMesh);

            foreach (var child in childs)
            {
                child.Visible = false;
            }

            Drawable.Parent = this;
        }

        private void updateDebugInformation()
        {
            System.Numerics.Vector3? target = null;

            if (GameObject.CurrentTask is Follow follow)
            {
                target = follow.Target.Transform.Position;
            }

            if (GameObject.CurrentTask is Move move)
            {
                target = move.Target;
            }

            if (GameObject.CurrentTask is Attack attack)
            {
                target = attack.Target.Transform.Position;
            }

            if (target.HasValue)
            {
                (TargetLineMesh.Geometry as LineGeometry)?.Update(GameObject.Transform.Position.ToGlm(), target.Value.ToGlm());
            }
            
//            (OrientationMesh.Geometry as LineGeometry)?.Update(GameObject.Position.ToGlm(), GameObject.Position.ToGlm() + 3.0f * GameObject.Orientation.ToGlm());

            HitBoxMesh.Position = GameObject.Transform.Position.ToGlm();
        }

        public vec3 Position
        {
            get => GameObject.Transform.Position.ToGlm();
            set {}
        }

        public vec3 Rotation
        {
            get => new vec3(1.0f, 0.0f, 0.0f); 
            set {}
        }

        public vec3 Scale { get => Drawable.Scale;
            set { Drawable.Scale = value; }
        }

        private float map(float c)
        {
            float start1 = -1.0f;
            float stop1 = 1.0f;

            float start2 = -(float)Math.PI;
            float stop2 = (float)Math.PI;
            return start2 + (stop2 - start2) * ((c - start1) / (stop1 - start1));
        }
        
        public mat4 GetModelMatrix()
        {
//            var m = mat4.identity();

            var m = glm.inverse(glm.lookAt(Position, Position + 30.0f * Rotation, new vec3(0.0f, 1.0f, 0.0f)));
            m = glm.rotate(m, (float)- Math.PI / 2.0f, new vec3(0.0f, 1.0f, 0.0f));
            m = glm.scale(m, Scale);

            return m;
        }

        public void Update(float deltaTime)
        {
            updateDebugInformation();
        }

        public IEnumerable<Mesh> GetAllMeshes()
        {
            var l = Drawable.GetAllMeshes();
                l = l.Concat(childs);

            return l;

        }

        public IEnumerable<ShaderProgram> GetAllShaders()
        {
            return Drawable.GetAllShaders().Concat(childs.Select(s => s.Material.Program).Distinct());
        }
    }
}