using GameEngine.Scenes;
using GameEngine.Utility;
using OpenGL_Game.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Input;
using System.Linq;

namespace OpenGL_Game.Scenes
{
    class HighScoreScene : Scene
    {
        List<int> Scores; 

        public HighScoreScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Options";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            //Load the GUI
            GUI.SetUpGUI(sceneManager.ClientSize.Width, sceneManager.ClientSize.Height, Vector2.Zero);

            this.sceneManager.KeyDown += this.SceneManager_KeyDown;

            Scores = new List<int>(5);
            LoadScores(); 
        }

        private void SceneManager_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Space:
                    this.Close();
                    this.sceneManager.StartMenu();
                    break;
            }
        }

        private void LoadScores()
        {
            var path = @"Data\Scores.txt";

            if (!File.Exists(path))
                return;

            string[] filecontents = File.ReadAllText(path).Split('\n');

            if (filecontents[0] == string.Empty)
                return; 

            List<int> scores = new List<int>();
            foreach(var line in filecontents)
            {
               if (int.TryParse(line, out int s))
                    scores.Add(s); 
            }

            scores.Sort();

            Scores = scores.Skip(Math.Max(0, scores.Count - 5)).Reverse().ToList();
        }       

        public override void Update(FrameEventArgs e)
        {
        }

        public override void Render(FrameEventArgs e)
        {
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, sceneManager.Width, 0, sceneManager.Height, -1, 1);

            GUI.clearColour = Color.DarkSeaGreen;

            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, 100, (int)width, (int)(fontSize * 2f)), "HighScores", 40, StringAlignment.Center);

            int divY = 45;
            for(int i = 0; i < Scores.Count; i++)
            {
                var score = Scores[i];
                GUI.Label(new Rectangle(0, 250 + (divY * i), (int)width, (int)(fontSize * 2f)), score.ToString(), 20, StringAlignment.Center);

            }

            GUI.Label(new Rectangle(0, 750, (int)width, 20), "press space to return to main menu", 10, StringAlignment.Center, Color.Green);
            GUI.Render();
        }

        public override void Close()
        {
            this.sceneManager.KeyDown -= this.SceneManager_KeyDown;
        }
    }
}