using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Geometry
{
    public class Pyramid : Shape
    {
        public Pyramid(float radius, float height)
        {            
            this.Radius = radius;
            this.Height = height;

            VerticesCount = GetVertices().Length;
            IndicesCount = GetIndices().Length;
        }

        public override Vector3[] GetVertices()
        {
            return new Vector3[]
            {
                new Vector3(-Radius, 0, -Radius),
                new Vector3(Radius, 0, -Radius),
                new Vector3(Radius, 0, Radius),
                new Vector3(-Radius, 0, Radius),
                new Vector3(0, Height, 0),
            };
        }

        public override int[] GetIndices(int offset = 0)
        {
            int[] indices = new int[]
            {
                // back face
                0, 1, 4,
                // front face
                3, 4, 2,
                // left face 
                0, 4, 3,
                // right face 
                2, 4, 1,
                // bottom face
  
            };

            if (offset != 0)
            {
                for (int i = 0; i < indices.Length; i++)
                {
                    indices[i] += offset;
                }
            }

            return indices;
        }

        public override Vector2[] GetTextureCoords()
        {
            return new Vector2[]
            {
                // left
                new Vector2(0.0f, 0.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
 
                // back
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
 
                // right
                new Vector2(-1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                 // front
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
 
                // bottom
                /*
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f)
                */
            };
        }

        public override Vector3[] GetColorData()
        {
            throw new NotImplementedException();
        }
    }
}
