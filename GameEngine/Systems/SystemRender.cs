using GameEngine.Components;
using GameEngine.Entities;
using GameEngine.Geometry;
using GameEngine.Managers;
using GameEngine.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;

namespace GameEngine.Systems
{
    public class SystemRender : ISystem
    {
        private const ComponentTypes MASK = ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TEXTURE;
        private const ComponentTypes SKY_MASK = ComponentTypes.COMPONENT_GEOMETRY | ComponentTypes.COMPONENT_TEXTURE;

        public ShaderUtility shader_main, shader_cubemap, shader_colour, shader_skybox, shader_GUI;

        public SystemRender(string vertShader, string fragShader)
        {
            shader_main = new ShaderUtility(vertShader, fragShader);
            shader_skybox = new ShaderUtility("skybox.vert", "skybox.frag");
            shader_cubemap = new ShaderUtility("cubemap.vert", "cubemap.frag");
            shader_colour = new ShaderUtility("default.vert", "default.frag");
            shader_GUI = new ShaderUtility("GUI.vert", "texture.frag"); 
        }

        void LoadShader(string filename, ShaderType type, int program, out int address)
        {
            address = GL.CreateShader(type);
            using (StreamReader sr = new StreamReader(filename))
            {
                GL.ShaderSource(address, sr.ReadToEnd());
            }
            GL.CompileShader(address);
            GL.AttachShader(program, address);
            Console.WriteLine(GL.GetShaderInfoLog(address));
        }

        public ShaderUtility Shader { get { return shader_main; } }

        public string Name
        {
            get { return "SystemRender"; }
        }

        public int ProgramID
        {
            get { return shader_main.ProgramID; }
        }

        public void OnAction(Entity entity)
        {
        }

        public void RenderFrame(Entity entity, Matrix4 view)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                var componentGeometry = (ComponentGeometry)entity.GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
                var componentPosition = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION);
                var componentTexture = (ComponentTexture)entity.GetComponent(ComponentTypes.COMPONENT_TEXTURE);


                BindData(shader_main, componentGeometry._Object, componentPosition.Position);

                Draw(shader_main, componentGeometry._Object, componentTexture.Texture, componentTexture.TextureTarget, view);
            }
            //else if ((entity.Mask & SKY_MASK) == SKY_MASK)
            //{
            //    var componentGeometry = (ComponentGeometry)entity.GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            //    var componentTexture = (ComponentTexture)entity.GetComponent(ComponentTypes.COMPONENT_TEXTURE);

            //    BindData(shader_skybox, componentGeometry._Object);
            //    Draw(shader_skybox, componentGeometry._Object, componentTexture.Texture, componentTexture.TextureTarget, view);

            //}
        }

        public void BindData(ShaderUtility shader, Shape shape, Vector3 position)
        {
            GL.UseProgram(shader.ProgramID);


            DataPipeline.BindVector3ArrayBuffer(shader, "vPosition", shape.GetVertices(),
                BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, false);

            if (shader.GetAttribute("vColor") != -1)
            {
                DataPipeline.BindVector3ArrayBuffer(shader, "vColor", shape.GetColorData(),
                    BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, true);
            }

            if (shader.GetAttribute("texcoord") != -1)
            {
                DataPipeline.BindVector2ArrayBuffer(shader, "texcoord", shape.GetTextureCoords(),
                    BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, true);
            }
            shape.CalculateModelMatrix(position);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ResourceManager.gl_buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(shape.IndicesCount * sizeof(int)), shape.GetIndices(), BufferUsageHint.StaticDraw);
        }

        public void BindData(ShaderUtility shader, Shape shape)
        {
            GL.UseProgram(shader.ProgramID);


            DataPipeline.BindVector3ArrayBuffer(shader, "vPosition", shape.GetVertices(),
                BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, false);

            if (shader.GetAttribute("vColor") != -1)
            {
                DataPipeline.BindVector3ArrayBuffer(shader, "vColor", shape.GetColorData(),
                    BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, true);
            }

            if (shader.GetAttribute("texcoord") != -1)
            {
                DataPipeline.BindVector2ArrayBuffer(shader, "texcoord", shape.GetTextureCoords(),
                    BufferTarget.ArrayBuffer, BufferUsageHint.StaticDraw, VertexAttribPointerType.Float, true);
            }
            shape.CalculateModelMatrix(Vector3.Zero);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ResourceManager.gl_buffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(shape.IndicesCount * sizeof(int)), shape.GetIndices(), BufferUsageHint.StaticDraw);
        }

        public void Draw(ShaderUtility shader, Shape shape, int texture, TextureTarget textureTarget, Matrix4 view)
        {
            GL.UseProgram(shader.ProgramID);
            shader_main.EnableVertexAttribArrays();

            DataPipeline.UniformShape(shader_main, shape, texture, ref view);
            GL.DrawElements(shape.RenderType, shape.IndicesCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));

            GL.BindTexture(textureTarget, 0);
            shader_main.DisableVertexAttribArrays();

        }

        public void RenderSkybox(Matrix4 view, Entity skybox)
        {
            var geoComponent = (ComponentGeometry)skybox.GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            var texComponent = (ComponentTexture)skybox.GetComponent(ComponentTypes.COMPONENT_TEXTURE);
            int texture = texComponent.Texture;

            GL.UseProgram(ProgramID);
            shader_main.EnableVertexAttribArrays();

            DataPipeline.UniformShape(shader_main, geoComponent._Object, texture, ref view);
            GL.DrawElements(geoComponent._Object.RenderType, geoComponent._Object.IndicesCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));

            GL.BindTexture(texComponent.TextureTarget, 0);
            shader_main.DisableVertexAttribArrays();


            //GL.Disable(EnableCap.DepthTest);
            //var componentGeometry = (ComponentGeometry)skybox.GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            //var componentTexture = (ComponentTexture)skybox.GetComponent(ComponentTypes.COMPONENT_TEXTURE);

            //int texId = componentTexture.Texture;
            //GL.UseProgram(shader_cubemap.ProgramID);


            //GL.BindVertexArray(skybuffer);
            //shader_cubemap.EnableVertexAttribArrays();
            //GL.BindTexture(TextureTarget.TextureCubeMap, texId);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, ResourceManager.gl_skybuffer);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(componentGeometry._Object.GetVertices().Length * Vector3.SizeInBytes), componentGeometry._Object.GetVertices(), BufferUsageHint.StaticDraw);
            //GL.VertexAttribPointer(ResourceManager.gl_skybuffer, 3, VertexAttribPointerType.Float, false, 0, 0);



            //GL.DrawArrays(PrimitiveType.Triangles, 0, componentGeometry._Object.VerticesCount);


            //shader_cubemap.DisableVertexAttribArrays();
            //GL.Enable(EnableCap.DepthTest);

        }
    }
}
