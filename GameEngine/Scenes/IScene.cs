using OpenTK;

namespace GameEngine.Scenes
{
    interface IScene
    {
        void Render(FrameEventArgs e);
        void Update(FrameEventArgs e);
        void Close();
    }
}
