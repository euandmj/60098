using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Geometry
{
    public class Model : Shape
    {
        public List<Tuple<Vertex, Vertex, Vertex>> Faces = new List<Tuple<Vertex, Vertex, Vertex>>();

        // Point data
        public Vector3[] Vertices { get; set; }
        public Vector3[] Colors { get; set; }
        public Vector2[] TexCoordinates { get; set; }

        public string Name { get; set; }

        public override int VerticesCount { get { return Faces.Count * 3; } }
        public override int IndicesCount { get { return Faces.Count * 3; } }
        public override int ColorDataCount { get { return Faces.Count * 3; } }
        public override int TextureCoordsCount { get { return Faces.Count * 3; } }

        public Model(string modelName)
        {
            Name = modelName;
        }

        public Model() { }      

        public override Vector3[] GetColorData()
        {
            return new Vector3[] {
                new Vector3(1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f)
            };
        }

        public override int[] GetIndices(int offset = 0)
        {
            return Enumerable.Range(offset, IndicesCount).ToArray();
        }

        public override Vector2[] GetTextureCoords()
        {
            List<Vector2> coords = new List<Vector2>();

            foreach (var face in Faces)
            {
                coords.Add(face.Item1.TextureCoord);
                coords.Add(face.Item2.TextureCoord);
                coords.Add(face.Item3.TextureCoord);
            }

            return coords.ToArray();
        }

        public override Vector3[] GetVertices()
        { 
            List<Vector3> verts = new List<Vector3>();

            foreach (var face in Faces)
            {
                verts.Add(face.Item1.Position);
                verts.Add(face.Item2.Position);
                verts.Add(face.Item3.Position);
            }

            return verts.ToArray();
        }
    }

    /// <summary>
    /// Holds vertices, color and texture data that has been loaded from a .obj file
    /// </summary>
    public struct Vertex
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;

        public Vertex(Vector3 pos, Vector3 norm, Vector2 texcoord)
        {
            Position = pos;
            Normal = norm;
            TextureCoord = texcoord;
        }
    }
}
