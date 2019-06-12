using System.Collections.Generic;

namespace GameRenderer
{
    public class Pipeline
    {
        private readonly List<Mesh> _depthPipeline = new List<Mesh>();
        private readonly List<Mesh> _transparentPipeline = new List<Mesh>();
        private readonly List<Mesh> _mainPipeline = new List<Mesh>();
        
        public Pipeline(IEnumerable<IDrawable> drawables)
        {
            foreach (var drawable in drawables)
            {
                var l = drawable.GetAllMeshes();
                foreach (var mesh in l)
                {
                    if (!mesh.UseDepth)
                    {
                        _depthPipeline.Add(mesh);
                    }
                    else if (mesh.UseBlending)
                    {
                        _transparentPipeline.Add(mesh);
                    }
                    else
                    {
                        _mainPipeline.Add(mesh);    
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (var mesh in _depthPipeline)
            {
                mesh.Draw();
            }

            foreach (var mesh in _mainPipeline)
            {
                mesh.Draw();
            }

            foreach (var mesh in _transparentPipeline)
            {
                mesh.Draw();
            }
        }
    }
}