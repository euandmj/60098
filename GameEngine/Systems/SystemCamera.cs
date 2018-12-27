using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using System.Text;
using GameEngine.Entities;

namespace GameEngine.Systems
{
    public class SystemCamera : ISystem
    {
        public Vector3 Position = Vector3.Zero;
        public Vector3 Rotation = new Vector3((float)Math.PI, 0f, 0f);
        public float MoveSpeed = 0.2f;
        public float MouseSensitivity = 0.01f;

        public string Name {  get { return "SystemCamera"; } }

        public void OnAction(Entity entity)
        {
        }

        public Matrix4 GetViewMatrix()
        {
            Vector3 lookat = new Vector3
            {
                X = (float)(Math.Sin(Rotation.X) * Math.Cos(Rotation.Y)),
                Y = (float)Math.Sin(Rotation.Y),
                Z = (float)(Math.Cos(Rotation.X) * Math.Cos(Rotation.Y))
            };

            return Matrix4.LookAt(Position, Position + lookat, Vector3.UnitY);
        }
        
        public void Move(float x, float y, float z)
        {
            Vector3 offset = new Vector3();
            Vector3 yDelta = new Vector3((float)Math.Sin(Rotation.X), 0, (float)Math.Cos(Rotation.X));
            Vector3 XDelta = new Vector3(-yDelta.Z, 0, yDelta.X);

            offset += x * XDelta;
            offset += y * yDelta;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
        }

        public void Move(Vector3 vec)
        {
            Vector3 offset = new Vector3();
            Vector3 yDelta = new Vector3((float)Math.Sin(Rotation.X), 0, (float)Math.Cos(Rotation.X));
            Vector3 XDelta = new Vector3(-yDelta.Z, 0, yDelta.X);

            offset += vec.X * XDelta;
            offset += vec.Y * yDelta;
            offset.Y += vec.Z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            Position += offset;
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
