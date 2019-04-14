using System.Collections.Generic;
using GameModel;
using GameModel.Tasks;
using GlmNet;

namespace GameRenderer
{
    public class UnitMesh : Mesh
    {
        public Unit Unit;

        public bool Selected { get; set; } = false;

        private Mesh TargetLineMesh;

        private Mesh HitBoxMesh;

        private Mesh OrientationMesh;
        
        public UnitMesh(Geometry geometry, Material material, Unit unit) : base(geometry, material)
        {
            Unit = unit;
            var color = new vec4(unit.Player.Color.ToGlm(), 1.0f);
            HitBoxMesh = new Mesh(new SphereGeometry(Unit.Radius, color),
                new ColorMaterial());
            
            TargetLineMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 1.0f, 0.4f, 1.0f)), new ColorMaterial());

            OrientationMesh = new Mesh(new LineGeometry(new vec3(), new vec3(), new vec4(1.0f, 0.9f, 0.9f, 1.0f)), new ColorMaterial());
            
            childs = new List<Mesh>();
            childs.Add(TargetLineMesh);
            childs.Add(HitBoxMesh);
            childs.Add(OrientationMesh);
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
        
        public override vec3 Position => Unit.Position.ToGlm();
        
        public override vec3 Rotation => Unit.Orientation.ToGlm();

        public override void Draw()
        {
            base.Draw();
            if (Selected)
            {
                updateDebugInformation();
                foreach (var child in childs)
                {
                    child.Draw();
                }
            }
        }
    }
}