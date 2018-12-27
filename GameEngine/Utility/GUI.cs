using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using GameEngine.Systems;
using GameEngine.Utility;

namespace GameEngine.Utility
{
    public static class GUI
    {

        static private Bitmap textBMP; //The image being drawn
        static Bitmap TextBMP
        {
            get { return textBMP; }
            set { }
        }//Stop outside scripts from changing GUI

        static private int textTexture;//The location of the texture on the graphics card
        static private Graphics textGFX;//Used to adjust the textBMP bitmap

        static private int m_width, m_height;
        static public Vector2 guiPosition;

        static public Color clearColour = Color.CornflowerBlue;

        //Called by SceneManager onLoad, and when screen size is changed
        public static void SetUpGUI(int width, int height, Vector2 guiPos)
        {

            m_width = width;
            m_height = height;
            guiPosition = guiPos;

            // Create Bitmap
            textBMP = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb); // match window size
            //Setup the graphics
            textGFX = Graphics.FromImage(textBMP);
            textGFX.Clear(clearColour);

            //Load the texture into the Graphics Card
            if (textTexture > 0)
            {
                GL.DeleteTexture(textTexture);
                textTexture = 0;
            }
            textTexture = GL.GenTexture();
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, textTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, textBMP.Width, textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);
        }

        static public void Label(Rectangle rect, string text)
        {
            Label(rect, text, 20, StringAlignment.Near);
        }
        static public void Label(Rectangle rect, string text, StringAlignment sa)
        {
            Label(rect, text, 20, sa);
        }
        static public void Label(Rectangle rect, string text, int fontSize)
        {
            Label(rect, text, fontSize, StringAlignment.Near);
        }

        static public void Label(Rectangle rect, string text, int fontSize, StringAlignment sa)
        {
            Label(rect, text, fontSize, sa, Color.White);
        }

        static public void Label(Rectangle rect, string text, int fontSize, StringAlignment sa, Color color)
        {
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = sa;
            stringFormat.LineAlignment = sa;

            SolidBrush brush = new SolidBrush(color);

            textGFX.DrawString(text, new Font("Arial", fontSize), brush, rect, stringFormat);
        }

        static public void Render()
        {
            GL.UseProgram(0);
            // Enable the texture
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthClamp);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.BindTexture(TextureTarget.Texture2D, textTexture);

            BitmapData data = textBMP.LockBits(new Rectangle(0, 0, textBMP.Width, textBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)textBMP.Width, (int)textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            textBMP.UnlockBits(data);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(guiPosition.X, guiPosition.Y);
            GL.TexCoord2(1f, 1f); GL.Vertex2(guiPosition.X + m_width, guiPosition.Y);
            GL.TexCoord2(1f, 0f); GL.Vertex2(guiPosition.X + m_width, guiPosition.Y + m_height);
            GL.TexCoord2(0f, 0f); GL.Vertex2(guiPosition.X, guiPosition.Y + m_height);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);

            textGFX.Clear(clearColour);
        }

        static public void Render(ShaderUtility shader)
        {
            GL.UseProgram(shader.ProgramID);
            // Enable the texture
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.DepthClamp);
            GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Texture2D);
            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            GL.BindTexture(TextureTarget.Texture2D, textTexture);

            BitmapData data = textBMP.LockBits(new Rectangle(0, 0, textBMP.Width, textBMP.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)textBMP.Width, (int)textBMP.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            textBMP.UnlockBits(data);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 1f); GL.Vertex2(guiPosition.X, guiPosition.Y);
            GL.TexCoord2(1f, 1f); GL.Vertex2(guiPosition.X + m_width, guiPosition.Y);
            GL.TexCoord2(1f, 0f); GL.Vertex2(guiPosition.X + m_width, guiPosition.Y + m_height);
            GL.TexCoord2(0f, 0f); GL.Vertex2(guiPosition.X, guiPosition.Y + m_height);
            GL.End();

            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);

            textGFX.Clear(clearColour);
        }
    }
}

