using OpenTK;
using GameEngine.Managers;

namespace GameEngine.Scenes
{
    public abstract class Scene
    {
        protected SceneManager sceneManager;

        public Scene(SceneManager sceneManager)
        {
            this.sceneManager = sceneManager;
        }

        public abstract void Render(FrameEventArgs e);

        public abstract void Update(FrameEventArgs e);

        public abstract void Close();
    }
}
