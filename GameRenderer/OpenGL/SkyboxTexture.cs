using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using GameRenderer.OpenGL;
using OpenTK.Graphics.OpenGL4;

namespace GameRenderer
{
    public class SkyboxTexture : Texture
    {
        private readonly int _id;

        /*
            Correct order:
            Front,
            Back,
            Top,
            Bottom,
            Right,
            Left,
         */
        public SkyboxTexture(List<string> faces)
        {
            GL.GenTextures(1, out _id);
            GL.BindTexture(TextureTarget.Texture2D, _id);


            for (int i = 0; i < 6; i++)
            {
                var bitmap = new Bitmap(faces[i]);
                var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                GL.TexImage2D ((TextureTarget.TextureCubeMapPositiveX + i), 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height
                    , 0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
                
                bitmap.UnlockBits(data);
            }
            
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int) TextureWrapMode.ClampToEdge);
        }
        
        public void  Use(TextureUnit t)
        {
            GL.ActiveTexture(t);
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }
    }
}