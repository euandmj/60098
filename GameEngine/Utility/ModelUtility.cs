using GameEngine.Entities;
using GameEngine.Geometry;
using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameEngine.Utility
{
    internal struct TempVertex
    {
        public int Vertex;
        public int Normal;
        public int Texcoord;

        public TempVertex(int vert = 0, int norm = 0, int tex = 0)
        {
            Vertex = vert;
            Normal = norm;
            Texcoord = tex;
        }
    }

    class ModelUtility
    {
        public static Model LoadModel(string path)
        {

            string fileContents = string.Empty;
            string[] lines;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    fileContents = sr.ReadToEnd();
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"{e.Message}: \n{path}");
            }

            lines = fileContents.Split('\n');

            // Temporary vectors to hold object data.
            List<Vector3> _vertices = new List<Vector3>();
            List<Vector3> _normals = new List<Vector3>();
            List<Vector2> _textures = new List<Vector2>();
            var _faces = new List<Tuple<TempVertex, TempVertex, TempVertex>>();

            // IS THIS FUCKING ALL MODELS?
            _vertices.Add(new Vector3());
            _normals.Add(new Vector3());
            _textures.Add(new Vector2());


            foreach (string line in lines)
            {
                if (line.Length < 2) continue;
                string temp = line.Substring(2);
                switch (line.Substring(0, 2))
                {
                    case "v ":
                        _vertices.Add(ParseVerticesFromObj(temp));
                        break;
                    // vertex face
                    case "f ":
                        _faces.Add(ParseFaceFromObj(temp, _textures.Count, _normals.Count));
                        break;
                    // vertex normal
                    case "vn":
                        _normals.Add(ParseNormalsFromObj(line));
                        break;
                    // vertex texture
                    case "vt":
                        _textures.Add(ParseTextureFromObj(line));
                        break;
                }
            }

            Model model = new Model(path);

            foreach (var face in _faces)
            {
                // Builds faces from the tuple.
                Vertex v0 = new Vertex(_vertices[face.Item1.Vertex], _normals[face.Item1.Normal], _textures[face.Item1.Texcoord]);
                Vertex v1 = new Vertex(_vertices[face.Item2.Vertex], _normals[face.Item2.Normal], _textures[face.Item2.Texcoord]);
                Vertex v2 = new Vertex(_vertices[face.Item3.Vertex], _normals[face.Item3.Normal], _textures[face.Item3.Texcoord]);

                model.Faces.Add(new Tuple<Vertex, Vertex, Vertex>(v0, v1, v2));
            }


            return model;

        }

        private static Vector2 ParseTextureFromObj(string line)
        {
            Vector2 vec2 = new Vector2();
            bool isParseSuccess;

            string[] data = line.Split(' ');

            isParseSuccess = float.TryParse(data[0], out vec2.X);
            isParseSuccess |= float.TryParse(data[1], out vec2.Y);

            if (!isParseSuccess)
            {
                Console.WriteLine($"Error parsing a texture line. Line: {line}");
            }
            return vec2;
        }

        /* Parses a vector line in the obj file */
        private static Vector3 ParseVerticesFromObj(string line)
        {
            Vector3 vector = new Vector3();
            bool isParseSuccess;

            string[] data = line.Split(' ');

            isParseSuccess = float.TryParse(data[0], out vector.X);
            isParseSuccess |= float.TryParse(data[1], out vector.Y);
            isParseSuccess |= float.TryParse(data[2], out vector.Z);
            if (!isParseSuccess)
            {
                Console.WriteLine($"Error parsing a vertex line. Line: {line}");
            }
            return vector;
        }

        private static Vector3 ParseNormalsFromObj(string line)
        {
            Vector3 normal = new Vector3();
            bool isParseSuccess;
            string[] data = line.Split(' ');

            isParseSuccess = float.TryParse(data[0], out normal.X);
            isParseSuccess |= float.TryParse(data[1], out normal.Y);
            isParseSuccess |= float.TryParse(data[2], out normal.Z);

            if (!isParseSuccess)
            {
                Console.WriteLine($"Error parsing a normal line. Line: {line}");
            }
            return normal;
        }

        /* Parses a face line in the obj file, returns the face in a constructed tuple. */
        private static Tuple<TempVertex, TempVertex, TempVertex> ParseFaceFromObj(string line, int textCount, int normCount)
        {
            Tuple<TempVertex, TempVertex, TempVertex> face = new Tuple<TempVertex, TempVertex, TempVertex>
                (
                    new TempVertex(),
                    new TempVertex(),
                    new TempVertex()
                );
            bool isParseSuccess;
            string[] data = line.Split(' ');

            int v0, v1, v2;
            int t0, t1, t2;
            int n0, n1, n2;

            // Parse block for vertices values
            isParseSuccess = int.TryParse(data[0].Split('/')[0], out v0);
            isParseSuccess |= int.TryParse(data[1].Split('/')[0], out v1);
            isParseSuccess |= int.TryParse(data[2].Split('/')[0], out v2);

            if (data[0].Count((char c) => c == '/') >= 2)
            {
                // Parse block for texture and normal values
                isParseSuccess |= int.TryParse(data[0].Split('/')[1], out t0);
                isParseSuccess |= int.TryParse(data[1].Split('/')[1], out t1);
                isParseSuccess |= int.TryParse(data[2].Split('/')[1], out t2);

                isParseSuccess |= int.TryParse(data[0].Split('/')[2], out n0);
                isParseSuccess |= int.TryParse(data[1].Split('/')[2], out n1);
                isParseSuccess |= int.TryParse(data[2].Split('/')[2], out n2);
            }
            else
            {
                if (textCount > v0 &&
                    textCount > v1 &&
                    textCount > v2)
                {
                    t0 = v0;
                    t1 = v1;
                    t2 = v2;
                }
                else
                {
                    t0 = 0;
                    t1 = 0;
                    t2 = 0;
                }

                if (normCount > v0 &&
                    normCount > v1 &&
                    normCount > v2)
                {
                    n0 = v0;
                    n1 = v1;
                    n2 = v2;
                }
                else
                {
                    n0 = 0;
                    n1 = 0;
                    n2 = 0;
                }
            }

            if (!isParseSuccess)
            {
                Console.WriteLine($"Error parsing a face line. Line: {line}");
            }
            else
            {
                face = new Tuple<TempVertex, TempVertex, TempVertex>(new TempVertex(v0, n0, t0), new TempVertex(v1, n1, t1), new TempVertex(v2, n2, t2));
            }
            return face;
        }
    }
}
