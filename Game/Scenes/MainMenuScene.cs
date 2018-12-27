using GameEngine.Scenes;
using GameEngine.Utility;
using OpenGL_Game.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Input;

namespace OpenGL_Game.Scenes
{
    class MainMenuScene : Scene
    {
        private int MenuIndex = 0;
        private int MaxIndex = 0;
        List<string> MenuItems; 

        public MainMenuScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Main Menu";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            //Load the GUI
            GUI.SetUpGUI(sceneManager.ClientSize.Width, sceneManager.ClientSize.Height, Vector2.Zero);
            GUI.clearColour = Color.DarkSeaGreen;

            this.sceneManager.KeyPress += Keyboard_KeyPress;
            this.sceneManager.KeyDown += Keyboard_KeyDown;


            // menu options
            MenuItems = new List<string>
            {
                "Start Game",
                "Highscores", 
                "Options",
                "Exit Game"
            };
            MaxIndex = MenuItems.Count - 1; 
        }


        private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            if (sceneManager.Keyboard.KeyRepeat)
                return;
            switch (e.Key)
            {
                case Key.Up:
                case Key.W:
                    DecrementIndex();
                    break;
                case Key.Down:
                case Key.S:
                    IncrementIndex();
                    break;
                case Key.Space:
                    HandleSelection(MenuItems[MenuIndex]);
                    break;
            }
        }

        private void Keyboard_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void HandleSelection(string choice)
        {
            switch (choice)
            {
                case "Start Game":
                    this.Close();
                    this.sceneManager.StartNewGame();
                    break;
                case "Highscores":
                    this.Close();
                    this.sceneManager.StartScores();
                    break;
                case "Options":
                    this.Close();
                    this.sceneManager.StartOptions();
                    break;
                case "Exit Game":
                    this.sceneManager.Exit();
                    this.Close();
                    break;


            }
        }

        private void IncrementIndex()
        {
            MenuIndex = MenuIndex == MaxIndex ? 0 : MenuIndex + 1;
        }

        private void DecrementIndex()
        {
            MenuIndex = MenuIndex == 0 ? MaxIndex : MenuIndex - 1; 
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


            //Display the Title
            float width = sceneManager.Width, height = sceneManager.Height, fontSize = Math.Min(width, height) / 10f;
            GUI.Label(new Rectangle(0, 200, (int)width, (int)(fontSize * 2f)), "PacMan 3D", 45, StringAlignment.Center);

            for(int i = 0; i < MenuItems.Count; i++)
            {
                Color textcol = Color.White;

                if (i == MenuIndex)
                    textcol = Color.Green;

                GUI.Label(new Rectangle(0, 300 + (i * 45), (int)width, (int)(fontSize * 2f)), MenuItems[i], 15, StringAlignment.Center, textcol);


            }
            GUI.Label(new Rectangle(0, 500, (int)width, (int)(fontSize * 2f)), "Press Space To Select", 10, StringAlignment.Center);
            GUI.Render();
        }

        public override void Close()
        {
            this.sceneManager.KeyPress -= this.Keyboard_KeyPress;
            this.sceneManager.KeyDown -= this.Keyboard_KeyDown;
        }
    }
}