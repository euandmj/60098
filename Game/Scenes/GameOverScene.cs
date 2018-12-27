using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenGL_Game.Managers;
using System.IO;
using OpenTK.Input;
using GameEngine.Utility;
using GameEngine.Scenes;

namespace OpenGL_Game.Scenes
{
    class GameOverScene : Scene
    {
        public static float dt = 0f;
        private int playerScore;
        private int lives; 

        public GameOverScene(SceneManager sceneManager, int score, int livesleft) : base(sceneManager)
        {
            sceneManager.Title = "Game Over";
            sceneManager.renderer = Render;
            sceneManager.updater = Update;
            
            playerScore = score;
            lives = livesleft;

            this.sceneManager.Keyboard.KeyDown += Keyboard_KeyDown;

            AddScore();

            GUI.SetUpGUI(sceneManager.ClientSize.Width, sceneManager.ClientSize.Height, Vector2.Zero);
        }

        private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    // start scores screen because scene doesnt have gamewindow and 
                    // events carry on through into the next scene lol??
                    this.Close();
                    sceneManager.StartScores();
                    break;
            }
            
        }

        private void AddScore()
        {
            var path = @"Data\Scores.txt";

            if (!File.Exists(path))
            {
                File.Create(path).Dispose();
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(playerScore); 
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(playerScore); 
                }
            }
        }


        public override void Update(FrameEventArgs e)
        {
            dt += (float)e.Time;
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.Black;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 16f;

            string endOfGameMsg = lives > 0 ? "YOU WIN" : "YOU HAVE DIED";
            Color endColor = lives > 0 ? Color.Green : Color.Red; 


            GUI.Label(new Rectangle(0, 300, (int)width, (int)(fontSize * 2f)), endOfGameMsg, (int)fontSize, StringAlignment.Center, endColor);
            GUI.Label(new Rectangle(0, 500, (int)width, (int)(fontSize * 2f)), "YOUR SCORE: " + playerScore , (int)fontSize / 2, StringAlignment.Center, endColor);
            GUI.Label(new Rectangle(0, 750, (int)width, 15), "press space to return to main menu", 10, StringAlignment.Center, endColor);

            GUI.Render();
        }

        
        public override void Close()
        {
            this.sceneManager.Keyboard.KeyDown -= this.Keyboard_KeyDown;
        }
    }
}
