using GameEngine.Components;
using OpenTK;
using System;

namespace GameEngine.Managers
{
    public class CameraManager 
    {
        public float MouseSensitivity = 0.008f;
        public Vector3 Rotation = new Vector3((float)Math.PI, 0f, 0f);
        public Vector2 LastMousePosition { get; set; }
        public Matrix4 View { get; set; } = Matrix4.Identity;
        public Matrix4 Projection =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45), 800f / 480f, 0.01f, 100f);

        public CameraManager()
        {
            //ComponentPosition = new ComponentPosition(new Vector3(0, 0, 0)); 
        }

        public string Name {  get { return "SystemCamera"; } }

        public Matrix4 GetViewMatrix(ComponentPosition pos)
        {
            Vector3 lookat = new Vector3
            {
                X = (float)(Math.Sin(Rotation.X) * Math.Cos(Rotation.Y)),
                Y = (float)Math.Sin(Rotation.Y),
                Z = (float)(Math.Cos(Rotation.X) * Math.Cos(Rotation.Y))
            };

            return Matrix4.LookAt(pos.Position, pos.Position + lookat, Vector3.UnitY);
        }
        
        // old - moved to player
        public void Move(ref ComponentPosition pos, float x, float y, float z)
        {
            pos.LastPos = pos.Position;

            Vector3 offset = new Vector3();
            Vector3 yDelta = new Vector3((float)Math.Sin(Rotation.X), 0, (float)Math.Cos(Rotation.X));
            Vector3 XDelta = new Vector3(-yDelta.Z, 0, yDelta.X);

            offset += x * XDelta;
            offset += y * yDelta;
            offset.Y += z;

            offset.NormalizeFast();
            //offset = Vector3.Multiply(offset, MoveSpeed);

            pos.Position += offset;
        }

        // old 
        public void Move(ref ComponentPosition pos, Vector3 vec)
        {
            pos.LastPos = pos.Position;

            Vector3 offset = new Vector3();
            Vector3 yDelta = new Vector3((float)Math.Sin(Rotation.X), 0, (float)Math.Cos(Rotation.X));
            Vector3 XDelta = new Vector3(-yDelta.Z, 0, yDelta.X);

            offset += vec.X * XDelta;
            offset += vec.Y * yDelta;
            offset.Y += vec.Z;

            offset.NormalizeFast();
            //offset = Vector3.Multiply(offset, MoveSpeed);

            pos.Position += offset;

        }

        public void Rotate(float x, float y)
        {
            x *= MouseSensitivity;
            y *= MouseSensitivity;

            Rotation.X = (Rotation.X + x) % ((float)Math.PI * 2.0f);
            Rotation.Y = Math.Max(Math.Min(Rotation.Y + y, (float)Math.PI / 2.0f - 0.1f), (float)-Math.PI / 2.0f + 0.1f);
        }
    }
}
