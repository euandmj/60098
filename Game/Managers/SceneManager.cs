using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using GameEngine.Scenes;
using GameEngine.Utility;
using OpenGL_Game.Scenes;

namespace OpenGL_Game.Managers
{
    public class SceneManager : GameWindow
    {
        private const int width = 1600, height = 900;
        public float Sensitivity = 0.008f;
        Scene Scene;

        public delegate void SceneDelegate(FrameEventArgs e);
        public SceneDelegate renderer;
        public SceneDelegate updater;

        public SceneManager() : base(width, height, new OpenTK.Graphics.GraphicsMode(new OpenTK.Graphics.ColorFormat(8, 8, 8, 8), 16))
        {
            this.Keyboard.KeyDown += this.Keyboard_KeyDown;

        }

        public void StartNewGame()
        {
            Scene = new GameScene(this, Sensitivity);
        }

        public void StartMenu()
        {
            Scene = new MainMenuScene(this);
        }

        public void StartGameOver(int score, int livesleft)
        {
            Scene = new GameOverScene(this, score, livesleft);
        }

        public void StartOptions()
        {
            Scene = new OptionsScene(this); 
        }

        public void StartScores()
        {
            Scene = new HighScoreScene(this); 
        }

        private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    this.Scene.Close();
                    this.Exit();
                    break;

            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);;

            StartMenu(); 
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            updater(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            renderer(e);

            SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height);

            //Load the GUI
            GUI.SetUpGUI(Width, Height, Vector2.Zero);
        }
    }
}
