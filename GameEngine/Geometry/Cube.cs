using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using OpenTK.Graphics.OpenGL;
using GameEngine.Systems;

namespace GameEngine.Geometry
{
    public class Cube : Shape
    {
        // Default sizes as per GetVertices()
        const float DEFAULT_WIDTH = 1f;
        const float DEFAULT_DEPTH = 1f;

        public Cube()
        {            
            VerticesCount = GetVertices().Length;
            IndicesCount = GetIndices().Length;
            TextureCoordsCount = GetTextureCoords().Length;

            Width = DEFAULT_WIDTH;
            Depth = DEFAULT_DEPTH;
        }

        public Cube(Vector3 scalar)
        {

            VerticesCount = GetVertices().Length;
            IndicesCount = GetIndices().Length;
            TextureCoordsCount = GetTextureCoords().Length;

            Width = DEFAULT_WIDTH * scalar.X;
            Depth = DEFAULT_DEPTH * scalar.Z;

            this.Scale = scalar;
        }

        public override Vector3[] GetVertices()
        {
            return new Vector3[] {
                //front
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),

                //right
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  -0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),

                //back
                new Vector3(-0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),

                //top
                new Vector3(0.5f, 0.5f,  -0.5f), 
                new Vector3(-0.5f, 0.5f,  -0.5f), 
                new Vector3(0.5f, 0.5f,  0.5f), 
                new Vector3(-0.5f, 0.5f,  0.5f), 

                //left
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(-0.5f, 0.5f,  0.5f),
                new Vector3(-0.5f, 0.5f,  -0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f),

                //bottom
                new Vector3(-0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  -0.5f),
                new Vector3(0.5f, -0.5f,  0.5f),
                new Vector3(-0.5f, -0.5f,  0.5f)

            };
        }

        public override int[] GetIndices(int offset = 0)
        {
            int[] inds = new int[] {
                //frront
                0,1,2,0,3,1,

                //right
                4,5,6,4,6,7,

                //back
                8,9,10,8,10,11,

                //top
                13,14,12,13,15,14,

                //left
                16,17,18,16,19,17,

                //bottom 
                20,21,22,20,22,23
            };

            if (offset != 0)
            {
                for (int i = 0; i < inds.Length; i++)
                {
                    inds[i] += offset;
                }
            }

            return inds;
        }

        public override Vector2[] GetTextureCoords()
        {
            return new Vector2[] {
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

                // top
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 0.0f),
                new Vector2(-1.0f, 1.0f),

                // front
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 0.0f),

                // bottom
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(-1.0f, 1.0f),
                new Vector2(-1.0f, 0.0f)
            };
        }

        public override Vector3[] GetColorData()
        {
            return new Vector3[] {
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f),
                new Vector3( 0f, 1f, 0f),
                new Vector3( 1f, 0f, 0f),
                new Vector3( 0f, 0f, 1f)
            };
        }
    }
}
