using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Geometry
{
    public class Sphere : Shape
    {
        double H_ANGLE = Math.PI / 180 * 72;
        double V_ANGLE = Math.Atan(1.0f / 2);

        List<double> vertices = new List<double>(12 * 3);
        int i1, i2;
        double z, xy;
        double hAngle1, hAngle2;

        public Sphere(float radius)
        {
            hAngle1 = -Math.PI / 2 - H_ANGLE / 2;
            hAngle2 = -Math.PI / 2;

            vertices[0] = 0;
            vertices[1] = 0;
            vertices[2] = radius;


            for(int i = 1; i <= 5; ++i)
            {
                i1 = i * 3;
                i2 = (i + 5) * 3;

                z = radius * Math.Sin(V_ANGLE);
                xy = radius * Math.Cos(V_ANGLE);

                vertices[i1] = xy * Math.Cos(hAngle1);
                vertices[i2] = xy * Math.Cos(hAngle2);
                vertices[i1 + 1] = xy * Math.Sin(hAngle1);
                vertices[i2 + 1] = xy * Math.Sin(hAngle2);
                vertices[i1 + 2] = z;
                vertices[i2 + 2] = -z;

                hAngle1 += H_ANGLE;
                hAngle2 += H_ANGLE;
            }

            i1 = 11 * 3; vertices[0] = 0;
            vertices[1] = 0;
            vertices[2] = -radius;
        }


        public override Vector3[] GetColorData()
        {
            throw new NotImplementedException();
        }

        public override int[] GetIndices(int offset = 0)
        {
            throw new NotImplementedException();
        }

        public override Vector2[] GetTextureCoords()
        {
            throw new NotImplementedException();
        }

        public override Vector3[] GetVertices()
        {
            throw new NotImplementedException();
        }
    }
}
