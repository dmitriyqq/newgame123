using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp;
using Assimp.Unmanaged;

namespace GameRenderer
{
    public class ModelGeometry
    {
        private List<Mesh> meshes;
        
        public ModelGeometry(string path)
        {
            AssimpContext importer = new AssimpContext();
            var scene = importer.ImportFile(path);

            foreach (var mesh in scene.Meshes)
            {
                processMesh(mesh);
            }
        }

        public void processMesh(AiMesh mesh)
        {
            var geometry = new AssimpGeometry();
            var data = new List<TextureVertex>();
            
            for (int i = 0; i < mesh.NumVertices; i++)
            {
                mesh.
                data.Add(new TextureVertex());
            }
            
            
            geometry.UpdateData(new List<TextureVertex>
            {
                
            }.ToRawArray());
            
            meshes.Add();
        }
    }
}