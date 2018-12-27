using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Geometry
{
    public abstract class Shape
    {
        public PrimitiveType RenderType = PrimitiveType.Triangles;
        public Vector3 Rotation = new Vector3();
        //public Vector3 Rotation = Vector3.Zero;
        public Vector3 Scale = Vector3.One;

        public virtual int VerticesCount { get; set; }
        public virtual int IndicesCount { get; set; }
        public virtual int ColorDataCount { get; set; }
        public virtual int TextureCoordsCount { get; set; }
        public float Width { get; set; }
        public float Depth { get; set; }
        public float Radius { get; set; }
        public float Height { get; set; }

        public Matrix4 ModelMatrix = Matrix4.Identity;
        public Matrix4 ViewProjectionMatrix = Matrix4.Identity;
        public Matrix4 ModelViewProjectionMatrix = Matrix4.Identity;


        public abstract Vector2[] GetTextureCoords();
        public abstract Vector3[] GetVertices();
        public abstract Vector3[] GetColorData();
        public abstract int[] GetIndices(int offset = 0);     

        public void CalculateModelMatrix(Vector3 pos)
        {
            //ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) * 
            //    Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z)) * Matrix4.CreateTranslation(pos);
            ModelMatrix = Matrix4.CreateScale(Scale) * Matrix4.CreateRotationX(Rotation.X) * Matrix4.CreateRotationY(Rotation.Y) *
               Matrix4.CreateRotationZ(Rotation.Z) * Matrix4.CreateTranslation(pos);
        }        

        public void LookAt(Vector3 directionalVector)
        {
            //Vector3 mZ = Vector3.Normalize(directionalVector - thisPos);
            //var x = Vector3.Dot(Vector3.UnitY, mZ);
            //var y = 

            //Rotation = Utility.Vec3Helper.FaceVector(directionalVector);
        }

        
    }
}
