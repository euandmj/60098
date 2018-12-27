using OpenTK;

namespace GameEngine.Geometry
{
    public class WrappedTexCube : Cube
    {
        private string[] Textures = { "right.jpg", "left.jpg", "top.jpg", "bottom.jpg", "back.jpg", "front.jpg" };

        public WrappedTexCube(Vector3 scalar)
        {
            this.Scale = scalar;
        }

        public WrappedTexCube(float scalar)
        {
            this.Scale = new Vector3(scalar);
        }
        public override Vector2[] GetTextureCoords()
        {
            return new Vector2[] {
                // front
                new Vector2(0.0f, 0.0f),
                new Vector2(0.2f, 1.0f),
                new Vector2(0.2f, 0.0f),
                new Vector2(0.0f, 1.0f),

                // right
                new Vector2(0.2f, 0.0f),
                new Vector2(0.2f, 1.0f),
                new Vector2(0.4f, 1.0f),
                new Vector2(0.4f, 0.0f),

                // back
                new Vector2(0.4f, 0.0f),
                new Vector2(0.6f, 0.0f),
                new Vector2(0.6f, 1.0f),
                new Vector2(0.4f, 1.0f),

                // top
                new Vector2(0.8f, 1.0f),
                new Vector2(0.6f, 1.0f),
                new Vector2(0.8f, 0.0f),
                new Vector2(0.6f, 0.0f),

                // left
                new Vector2(0.8f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.8f, 1.0f),
                new Vector2(1.0f, 0.0f),

                // bottom
                new Vector2(0.6f, 1.0f),
                new Vector2(0.8f, 1.0f),
                new Vector2(0.8f, 0.0f),
                new Vector2(0.6f, 0.0f),
            };
        }
    }

}
