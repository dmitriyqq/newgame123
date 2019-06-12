using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL4;
using PixelFormat = OpenTK.Graphics.OpenGL4.PixelFormat;

namespace GameRenderer.OpenGL
{
    public class Texture
    {
        private int _id;

        public Texture(){}
        
        public Texture(string path, TextureTarget target = TextureTarget.Texture2D)
        {
            var bitmap = new Bitmap(path);

            GL.GenTextures(1, out _id);
            GL.BindTexture(TextureTarget.Texture2D, _id);

            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            
            GL.TexImage2D (target, 0, PixelInternalFormat.Rgba, bitmap.Width, bitmap.Height
                , 0, PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            
            
            bitmap.UnlockBits(data);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);
            
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);
        }

        public void Use(TextureUnit t = TextureUnit.Texture0)
        {
            GL.ActiveTexture(t);
            GL.BindTexture(TextureTarget.Texture2D, _id);
        }

        ~Texture()
        {
            GL.DeleteTexture(_id);
        }
    }
}