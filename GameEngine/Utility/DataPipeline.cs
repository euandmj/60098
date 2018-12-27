using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System;
using GameEngine.Systems;
using GameEngine.Components;
using GameEngine.Geometry;

namespace GameEngine.Utility
{
    class DataPipeline
    {
        /// <summary>
        /// Binds a vector3 array of data to the gpu buffer.
        /// </summary>
        public static void BindVector3ArrayBuffer(ShaderUtility shader, string name, Vector3[] data, BufferTarget target, BufferUsageHint hint, VertexAttribPointerType type, bool normalised, int stride = 0, int offset = 0)
        {
            GL.BindBuffer(target, shader.GetBuffer(name));
            GL.BufferData(target, (IntPtr)(data.Length * Vector3.SizeInBytes), data, hint);
            GL.VertexAttribPointer(shader.GetAttribute(name), 3, type, normalised, stride, offset);
        }

        /// <summary>
        /// Binds a vector2 array of data to the gpu bufer.
        /// </summary>
        public static void BindVector2ArrayBuffer(ShaderUtility shader, string name, Vector2[] data, BufferTarget target, BufferUsageHint hint, VertexAttribPointerType type, bool normalised, int stride = 0, int offset = 0)
        {
            GL.BindBuffer(target, shader.GetBuffer(name));
            GL.BufferData(target, (IntPtr)(data.Length * Vector2.SizeInBytes), data, hint);
            GL.VertexAttribPointer(shader.GetAttribute(name), 2, type, normalised, stride, offset);
        }

        /// <summary>
        //  Binds various uniform matrices to a Shape's components.
        /// </summary>
        public static void UniformShape(ShaderUtility shader, Shape shape, int texture, ref Matrix4 view)
        {
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.UniformMatrix4(shader.GetUniform("modelview"), false, ref shape.ModelViewProjectionMatrix);

            if (shader.GetAttribute("maintexture") != -1)
            {
                GL.Uniform1(shader.GetAttribute("maintexture"), texture);
            }

            if (shader.GetUniform("view") != -1)
            {
                GL.UniformMatrix4(shader.GetUniform("view"), false, ref view);
            }

            if (shader.GetUniform("model") != -1)
            {
                GL.UniformMatrix4(shader.GetUniform("model"), false, ref shape.ModelMatrix);
            }

            
        }

        /// <summary>
        /// Binds material data to the shader
        /// </summary>
        public static void UniformMaterial(ShaderUtility shader, Material material)
        {
            if (shader.GetUniform("material_ambient") != -1)
            {
                GL.Uniform3(shader.GetUniform("material_ambient"), ref material.AmbientColor);
            }

            if (shader.GetUniform("material_diffuse") != -1)
            {
                GL.Uniform3(shader.GetUniform("material_diffuse"), ref material.DiffuseColor);
            }

            if (shader.GetUniform("material_specular") != -1)
            {
                GL.Uniform3(shader.GetUniform("material_specular"), ref material.SpecularColor);
            }

            if (shader.GetUniform("material_specularExponent") != -1)
            {
                GL.Uniform1(shader.GetUniform("material_specularExponent"), material.SpecularExponent);
            }
        }
    }   
}
