using System;
using System.Collections.Generic;
using GlmNet;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class TexturedParticleEngine : ParticleEngine
    {
        public int TVBO { get; private set; }

        public TexturedParticleEngine()
        {
            var vertexBufferData = new []{
                -0.5f, -0.5f, 0.0f, 
                0.5f, -0.5f, 0.0f,   
                -0.5f, 0.5f, 0.0f,  
                0.5f, 0.5f, 0.0f, 
            };

            var tBufferData = new[]
            {
                0.0f, 0.0f,
                1.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,
            };

            Material = new TexturedParticlesMaterial();
    
            Vao = GL.GenVertexArray();
            GL.BindVertexArray(Vao);
    
            BillboardVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, BillboardVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferData.Length * sizeof(float), vertexBufferData, BufferUsageHint.StaticDraw);

            TVBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, tBufferData.Length * sizeof(float), tBufferData, BufferUsageHint.StaticDraw);

            
            // The VBO containing the positions and sizes of the particles
            PositionVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, PositionVbo);
            // Initialize with empty (NULL) buffer : it will be updated later, each frame.
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
    
            // The VBO containing the colors of the particles
            ColorVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorVbo);
            // Initialize with empty (NULL) buffer : it will be updated later, each frame.
            GL.BufferData(BufferTarget.ArrayBuffer, MaxParticles * 4 * sizeof(float), IntPtr.Zero, BufferUsageHint.StreamDraw);
    
            // 1rst attribute buffer : vertices
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, BillboardVbo);
            GL.VertexAttribPointer(
                0, // attribute. No particular reason for 0, but must match the layout in the shader.
                3, // size
                VertexAttribPointerType.Float, // type
                false, // normalized?
                0, // stride
                0 // array buffer offset
            );
            
            GL.EnableVertexAttribArray(1);
            GL.BindBuffer(BufferTarget.ArrayBuffer, TVBO);
            GL.VertexAttribPointer(
                1, // attribute. No particular reason for 1, but must match the layout in the shader.
                2, // size : x + y + z + size => 4
                VertexAttribPointerType.Float, // type
                false, // normalized?
                0, // stride
                0// array buffer offset
            );
            
    
            // 2nd attribute buffer : positions of particles' centers
            GL.EnableVertexAttribArray(2);
            GL.BindBuffer(BufferTarget.ArrayBuffer, PositionVbo);
            GL.VertexAttribPointer(
                2, // attribute. No particular reason for 1, but must match the layout in the shader.
                2, // size : x + y + z + size => 4
                VertexAttribPointerType.Float, // type
                false, // normalized?
                0, // stride
                0 // array buffer offset
            );
    
            // 3rd attribute buffer : particles' colors
            GL.EnableVertexAttribArray(3);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorVbo);
            GL.VertexAttribPointer(
                3, // attribute. No particular reason for 1, but must match the layout in the shader.
                4, // size : r + g + b + a => 4
                VertexAttribPointerType.Float, // type
                true, // normalized? *** YES, this means that the unsigned char[4] will be accessible with a vec4 (floats) in the shader ***
                0, // stride
                0 // array buffer offset
            );
            GL.VertexAttribDivisor(0, 0); // particles vertices : always reuse the same 4 vertices -> 0
            GL.VertexAttribDivisor(1, 0); // particles vertices : always reuse the same 4 vertices -> 0
            GL.VertexAttribDivisor(2, 1); // positions : one per quad (its center) -> 1
            GL.VertexAttribDivisor(3, 1); // color : one per quad -> 1
            GL.BindVertexArray(0);
        }

        public void Update2(float[] positions, float[] color, int count)
        {
            Update(positions, color, count);
            ParticlesCount = count;
        }
    }
}