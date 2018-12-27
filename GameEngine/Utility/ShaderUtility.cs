using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;
using System.IO;

namespace GameEngine.Utility
{
    public class ShaderUtility
    {
        public int ProgramID = -1;
        public int VertexShaderID = -1;
        public int FragShaderID = -1;
        public int AttributeCount = 0;
        public int UniformCount = 0;

        Dictionary<string, AttributeInfo> Attributes = new Dictionary<string, AttributeInfo>();
        Dictionary<string, UniformInfo> Uniforms = new Dictionary<string, UniformInfo>();
        public Dictionary<string, uint> Buffers = new Dictionary<string, uint>();

        public ShaderUtility()
        {
            ProgramID = GL.CreateProgram();
        }

        public ShaderUtility(string vert, string frag)
        {
            ProgramID = GL.CreateProgram();

            ReadShader(vert, ShaderType.VertexShader);
            ReadShader(frag, ShaderType.FragmentShader);

            Link();
            GenBuffers();
        }

        public void ReadShader(string filename, ShaderType type)
        {
            using (StreamReader sr = new StreamReader("Shaders/" + filename))
            {
                if (type == ShaderType.VertexShader)
                {
                    LoadShader(sr.ReadToEnd(), type, out VertexShaderID);
                }
                else if (type == ShaderType.FragmentShader)
                {
                    LoadShader(sr.ReadToEnd(), type, out FragShaderID);
                }
            }
        }

        private void LoadShader(string fileContents, ShaderType type, out int shaderAddress)
        {
            shaderAddress = GL.CreateShader(type);
            GL.ShaderSource(shaderAddress, fileContents);
            GL.CompileShader(shaderAddress);
            GL.AttachShader(ProgramID, shaderAddress);
            Console.WriteLine(GL.GetShaderInfoLog(shaderAddress));
        }

        public void Link()
        {
            GL.LinkProgram(ProgramID);

            Console.WriteLine(GL.GetProgramInfoLog(ProgramID));

            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveAttributes, out AttributeCount);
            GL.GetProgram(ProgramID, GetProgramParameterName.ActiveUniforms, out UniformCount);

            for (int i = 0; i < AttributeCount; i++)
            {
                AttributeInfo info = new AttributeInfo();
                StringBuilder name = new StringBuilder(256);

                GL.GetActiveAttrib(ProgramID, i, 256, out int length, out info.size, out info.type, name);

                info.name = name.ToString();
                info.address = GL.GetAttribLocation(ProgramID, info.name);
                Attributes.Add(name.ToString(), info);
            }

            for (int i = 0; i < UniformCount; i++)
            {
                UniformInfo info = new UniformInfo();
                StringBuilder name = new StringBuilder(256);

                GL.GetActiveUniform(ProgramID, i, 256, out int length, out info.size, out info.type, name);

                info.name = name.ToString();
                info.address = GL.GetUniformLocation(ProgramID, info.name);
                Uniforms.Add(name.ToString(), info);
            }

        }

        public void GenBuffers()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                GL.GenBuffers(1, out uint buffer);

                Buffers.Add(Attributes.Values.ElementAt(i).name, buffer);
            }

            for (int i = 0; i < Uniforms.Count; i++)
            {
                GL.GenBuffers(1, out uint buffer);

                Buffers.Add(Uniforms.Values.ElementAt(i).name, buffer);
            }
        }

        public void EnableVertexAttribArrays()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                GL.EnableVertexAttribArray(Attributes.Values.ElementAt(i).address);
            }
        }

        public void DisableVertexAttribArrays()
        {
            for (int i = 0; i < Attributes.Count; i++)
            {
                GL.DisableVertexAttribArray(Attributes.Values.ElementAt(i).address);
            }
        }

        public int GetAttribute(string name)
        {
            return Attributes.ContainsKey(name) ? Attributes[name].address : -1;
        }

        public int GetUniform(string name)
        {
            return Uniforms.ContainsKey(name) ? Uniforms[name].address : -1;
        }

        public uint GetBuffer(string name)
        {
            return Buffers.ContainsKey(name) ? Buffers[name] : 0;
        }


        internal class UniformInfo
        {
            public string name = "";
            public int address = -1;
            public int size = 0;
            public ActiveUniformType type;
        }

        internal class AttributeInfo
        {
            public string name = "";
            public int address = -1;
            public int size = 0;
            public ActiveAttribType type;
        }
    }
}
