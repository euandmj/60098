using GameEngine.Scenes;
using GameEngine.Utility;
using OpenGL_Game.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Collections.Generic;
using OpenTK.Input;

namespace OpenGL_Game.Scenes
{
    class OptionsScene : Scene
    {
        private int MenuIndex = 0;
        private int MaxIndex = 0;
        List<string> MenuItems;
        List<string> SensOptions;
        int SensIndex = 1;

        public OptionsScene(SceneManager sceneManager) : base(sceneManager)
        {
            // Set the title of the window
            sceneManager.Title = "Options";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;

            //Load the GUI
            GUI.SetUpGUI(sceneManager.ClientSize.Width, sceneManager.ClientSize.Height, Vector2.Zero);
            GUI.clearColour = Color.DarkSeaGreen;

            this.sceneManager.KeyPress += SceneManager_KeyPress;
            this.sceneManager.KeyDown += this.SceneManager_KeyDown;


            // menu options
            MenuItems = new List<string>
            {
                "Return",
                "Sensitivity:    ",
                "Controls: WASD/Arrows - Mouse for look"

            };
            MaxIndex = MenuItems.Count - 1;

            SensOptions = new List<string>
            {
                "Low",
                "Medium",
                "High"
            };
        }

        private void SceneManager_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
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
                case Key.Left:
                case Key.A:
                    if (MenuIndex == 1)
                        ChangeSensitivity(-1);
                    break;
                case Key.Right:
                case Key.D:
                    if (MenuIndex == 1)
                        ChangeSensitivity(1);
                    break;
                case Key.Space:
                    HandleSelection(MenuItems[MenuIndex]);
                    break;
            }
        }

        private void SceneManager_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void ChangeSensitivity(int delta)
        {
            if(delta > 0)
            {
                if (SensIndex < SensOptions.Count - 1)
                    SensIndex++;
            }
            else
            {
                if (SensIndex > 0)
                    SensIndex--;
            }

            switch (SensIndex)
            {
                case 0:
                    sceneManager.Sensitivity = 0.0008f;
                    break;
                case 1:
                    sceneManager.Sensitivity = 0.008f;
                    break;
                case 2:
                    sceneManager.Sensitivity = 0.018f;
                    break;
            }

            Console.WriteLine(sceneManager.Sensitivity); 


        }

        private void HandleSelection(string choice)
        {
            switch (choice)
            {
                case "Return":
                    this.Close();
                    this.sceneManager.StartMenu();
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
                string msg = string.Empty;

                if (i == MenuIndex)
                    textcol = Color.Green;


                if (i == 1)
                    msg = "Sensitivity:    " + SensOptions[SensIndex];
                else
                    msg = MenuItems[i];

                GUI.Label(new Rectangle(0, 300 + (i * 45), (int)width, (int)(fontSize * 2f)), msg, 15, StringAlignment.Center, textcol);


            }
            GUI.Label(new Rectangle(0, 500, (int)width, (int)(fontSize * 2f)), "Press Space To Select", 10, StringAlignment.Center);
            GUI.Render();
        }

        public override void Close()
        {
            this.sceneManager.KeyPress -= this.SceneManager_KeyPress;
            this.sceneManager.KeyDown -= this.SceneManager_KeyDown;
        }
    }
}