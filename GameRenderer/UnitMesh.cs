using System;
using System.Collections.Generic;
using System.Linq;
using GameModel;
using GameModel.Tasks;
using GlmNet;
using OpenTK;

namespace GameRenderer
{
    public class UnitMesh : IDrawable
    {
        public Unit Unit;

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

        public UnitMesh(IDrawable drawable, Unit unit)
        {
            Drawable = drawable;
            Unit = unit;

            if (unit.Player == null)
            {
                throw new Exception("No player for mesh");
            }
            
            var color = new vec4(unit.Player.Color.ToGlm(), 0.3f);
            HitBoxMesh = new Mesh(new SphereGeometry(Unit.Radius, color),
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
            Vector target = null;

            if (Unit.CurrentTask is Follow follow)
            {
                target = follow.Target.Position;
            }

            if (Unit.CurrentTask is Move move)
            {
                target = move.Target;
            }

            if (Unit.CurrentTask is Attack attack)
            {
                target = attack.Target.Position;
            }

            if (target != null)
            {
                (TargetLineMesh.Geometry as LineGeometry)?.Update(Unit.Position.ToGlm(), target.ToGlm());
            }
            
            (OrientationMesh.Geometry as LineGeometry)?.Update(Unit.Position.ToGlm(), Unit.Position.ToGlm() + 3.0f * Unit.Orientation.ToGlm());

            HitBoxMesh.Position = Unit.Position.ToGlm();
        }

        public vec3 Position
        {
            get => Unit.Position.ToGlm();
            set {}
        }

        public vec3 Rotation
        {
            get => Unit.Orientation.ToGlm(); 
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