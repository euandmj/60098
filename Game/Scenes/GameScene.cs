using GameEngine.Components;
using GameEngine.Entities;
using GameEngine.Geometry;
using GameEngine.Managers;
using GameEngine.Scenes;
using GameEngine.Systems;
using GameEngine.Utility;
using OpenGL_Game.Entities;
using OpenGL_Game.Managers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace OpenGL_Game.Scenes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class GameScene : Scene
    {
        readonly float[,] Nodes;
        // links to which index the node is at 
        readonly int[] testPointers = new int[]
        {
            7, 24, 7, 24
        };
        readonly int[] pinkNodeptr = new int[]
        {
            1, 12, 23, 24, 13, 7, 11
        };
        readonly int[] orangeNodePtr = new int[]
        {
            11, 1, 7, 10,
        };
        readonly int[] blueNodePtr = new int[]
        {
            8, 18, 21, 3, 20,
        };
        readonly int[] redNodePtr = new int[]
        {
            13, 22, 18, 12, 2,
        };

        public static float dt = 0;
        public static float TimeElapsed = 0;

        Random random; 
        CameraManager camera;
        SystemManager systemManager;
        RespawnManager respawner;
        EntityManager entityManager;
        AIPathingManager ghostPathingmgr;

        bool DEBUG_COLLISION_ENABLED = true;
        bool DEBUG_GHOST_ENABLED = true;

        public static GameScene gameInstance;

        public GameScene(SceneManager sceneManager, float sens = 0.008f) : base(sceneManager)
        {
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Viewport(0, 0, sceneManager.Width, sceneManager.Height);
            GL.ClearColor(Color.White);
            sceneManager.CursorVisible = false;
            GL.Enable(EnableCap.DepthTest);
            //GL.FrontFace(FrontFaceDirection.Ccw); 
            //GL.Enable(EnableCap.CullFace);
            
            GL.Frustum(-25, 25, -25, 25, 0.1, 55);
            //GL.Ortho(10, 10, 10, 10, 10, 10);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthClamp);
            //GL.PointSize(5f);

            random = new Random(); 
            gameInstance = this;
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            respawner = new RespawnManager(6);

            // Set the title of the window
            sceneManager.Title = "Game";
            // Set the Render and Update delegates to the Update and Render methods of this class
            sceneManager.renderer = Render;
            sceneManager.updater = Update;


            sceneManager.KeyDown += Keyboard_KeyPressed;
            sceneManager.Keyboard.KeyDown += Keyboard_KeyDown;


            ResourceManager.gl_buffer = GL.GenBuffer();
            ResourceManager.gl_skybuffer = GL.GenBuffer();


            GUI.SetUpGUI(250, 50, new Vector2(sceneManager.ClientSize.Width / 2, sceneManager.ClientSize.Height - 100));
            GUI.clearColour = Color.Red;

            camera = new CameraManager();
            camera.MouseSensitivity = sens;
            CreateSystems();
            ResourceManager.LoadAudio("death.wav");
            ResourceManager.LoadAudio("teleport.wav");
            ResourceManager.LoadAudio("willhelm.wav");
            ResourceManager.LoadAudio("beginning.wav");
            ResourceManager.LoadAudio("eatfruit.wav");
            ResourceManager.LoadAudio("eatghost.wav");
            ResourceManager.LoadAudio("siren.wav");

            Nodes = MapUtility.NodeLoader("GameArgs/AiNodes.txt");
            ghostPathingmgr = new AIPathingManager(Nodes);
            InitaliseGhostPathingNodes();
            CreateEntities();


            GUI.SetUpGUI(250, 50, new Vector2(sceneManager.ClientSize.Width / 2, 200));
            GUI.clearColour = Color.Green;


            Console.WriteLine("Debug Control:\nG - Toggle Ghosts\nC - Toggle Collision\n5 - Reset Position"); 
        }

        void UpdateMouseCursor()
        {
            var state = Mouse.GetState();
            Vector2 delta = camera.LastMousePosition - new Vector2(state.X, state.Y);

            //Camera SC = entityManager.GetPlayerCamera();
            camera.Rotate(delta.X, delta.Y);
            ResetMouseCursor();
        }

        void InitGame()
        {
        }


        void ResetMouseCursor()
        {
            Mouse.SetPosition(sceneManager.Width / 2, sceneManager.Height / 2);
            camera.LastMousePosition = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
        }

        private void LoadWalls()
        {
            List<Entity> walls = new List<Entity>();
            string texture = "wall.png";
            string textureLong = "wall_large.png";

            // Acw spec heights            
            float y = -.9f;
            float h = 2f;

            // better suited heights
            y = 1f;
            h = 2.5f;

            walls.Add(MapUtility.LoadWall(-15, y, -25, 20, h, 1, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(15, y, -25, 20, h, 1, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(-12.5f, y, 25, 25, h, 1, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(12.5f, y, 25, 25, h, 1, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(25, y, -15, 1, h, 20, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(-25, y, -15, 1, h, 20, textureLong, walls.Count));
            walls.Add(MapUtility.LoadWall(0, y, -22.5f, 10, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-25, y, 12.5f, 1, h, 25, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(25, y, 12.5f, 1, h, 25, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-5, y, -10, 1, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(5, y, -10, 1, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(0, y, -5, 11, h, 1, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-15, y, -12.5f, 10, h, 15, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(15, y, -17.5f, 10, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-15, y, 2.5f, 10, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(17.5f, y, -7.5f, 5, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(12.5f, y, -2.5f, 5, h, 15, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(17.5f, y, 2.5f, 5, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(2.5f, y, 2.5f, 5, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-2.5f, y, 5, 5, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-17.5f, y, 11.5f, 5, h, 3f, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-12.5f, y, 15, 5, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-17.5f, y, 18.5f, 5, h, 3f, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(0, y, 17.5f, 10, h, 5, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(7.5f, y, 15, 5, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(17.5f, y, 15, 5, h, 10, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(-3.5f, y, -15, 4, h, 1, texture, walls.Count));
            walls.Add(MapUtility.LoadWall(3.5f, y, -15, 4, h, 1, texture, walls.Count));

            walls.Add(MapUtility.LoadGhostSpawnWall(0, y, -15, 3, h, 1, "green.jpg", "spawnwall"));
            walls.Add(MapUtility.LoadWallPortal(-25f, y, -2.5f, 1, h, 5, "portal.png", "leftPortal"));
            walls.Add(MapUtility.LoadWallPortal(25f, y, -2.5f, 1, h, 5, "portal.png", "rightPortal"));

            entityManager.AddEntities(walls);

        }

        private void LoadPacDots()
        {
            List<Entity> pacdots = new List<Entity>();
            string name = $"pacdot_";
            Random rand = new Random(); 

            for (float z = -22.5f; z < 25; z += 2.5f)
            {
                for (float x = -22.5f; x < 25; x += 2.5f)
                {
                    if (x < 5 && x > -5 && z > -15 && z < -5) continue; // spawn room


                    if (IsPacdotInWall(new Vector3(x, .5f, z)) == false)
                    {
                        pacdots.Add(MapUtility.LoadPacdot(x, .75f, z, pacdots.Count));
                    }
                }
            }

            pacdots.RemoveAll(s => s.Name == "pacdot_59");
            entityManager.AddEntities(pacdots);
        }

        private bool IsPacdotInWall(Vector3 position)
        {
            var collision = (SystemCollision)systemManager.FindSystem("SystemCollision");
            bool isColl = false;

            // remove pacdots on walls and fruit
            foreach (var wall in entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_WALL || s.Entity_Type == EntityType.ENTITY_FRUIT))
            {
                isColl |= collision.CalculateCollision(position, wall);

                if (isColl)
                    return true;

            }
            return false;
        }

        private void InitaliseGhostPathingNodes(string path = null)
        {
            // maybe load them by file

            ghostPathingmgr.AddNode(0, new Dictionary<int, int>() { { 4, 10 } });
            ghostPathingmgr.AddNode(1, new Dictionary<int, int>() { { 2, 15 }, { 12, 20 } });
            ghostPathingmgr.AddNode(2, new Dictionary<int, int>() { { 1, 15 }, { 3, 5 } });
            ghostPathingmgr.AddNode(3, new Dictionary<int, int>() { { 2, 5 }, { 4, 5 }, { 11, 15 } });
            ghostPathingmgr.AddNode(4, new Dictionary<int, int>() { { 3, 5 }, { 5, 5 } });
            ghostPathingmgr.AddNode(5, new Dictionary<int, int>() { { 4, 5 }, { 6, 5 }, { 9, 5 } });
            ghostPathingmgr.AddNode(6, new Dictionary<int, int>() { { 5, 5 }, { 7, 15 } });
            ghostPathingmgr.AddNode(7, new Dictionary<int, int>() { { 6, 15 }, { 8, 10 } });
            ghostPathingmgr.AddNode(8, new Dictionary<int, int>() { { 7, 10 }, { 9, 15 } });
            ghostPathingmgr.AddNode(9, new Dictionary<int, int>() { { 8, 15 }, { 5, 5 }, { 10, 10 } });
            ghostPathingmgr.AddNode(10, new Dictionary<int, int>() { { 9, 10 }, { 11, 15 }, { 16, 10 } });
            ghostPathingmgr.AddNode(11, new Dictionary<int, int>() { { 10, 15 }, { 3, 15 }, { 12, 15 }, { 14, 10 } });
            ghostPathingmgr.AddNode(12, new Dictionary<int, int>() { { 11, 15 }, { 1, 20 }, { 13, 15 } });
            ghostPathingmgr.AddNode(13, new Dictionary<int, int>() { { 12, 15 }, { 14, 15 }, { 21, 15 } });
            ghostPathingmgr.AddNode(14, new Dictionary<int, int>() { { 13, 15 }, { 11, 10 }, { 19, 5 } });
            ghostPathingmgr.AddNode(15, new Dictionary<int, int>() { { 20, 5 }, { 16, 5 } });
            ghostPathingmgr.AddNode(16, new Dictionary<int, int>() { { 15, 5 }, { 10, 10 }, { 17, 5 } });
            ghostPathingmgr.AddNode(17, new Dictionary<int, int>() { { 16, 5 }, { 18, 10 } });
            ghostPathingmgr.AddNode(18, new Dictionary<int, int>() { { 17, 10 }, { 8, 20 }, { 24, 15 } });
            ghostPathingmgr.AddNode(19, new Dictionary<int, int>() { { 14, 5 }, { 20, 10 }, { 22, 10 } });
            ghostPathingmgr.AddNode(20, new Dictionary<int, int>() { { 19, 10 }, { 15, 5 } });
            ghostPathingmgr.AddNode(21, new Dictionary<int, int>() { { 13, 15 }, { 22, 15 } });
            ghostPathingmgr.AddNode(22, new Dictionary<int, int>() { { 21, 15 }, { 19, 10 }, { 23, 20 } });
            ghostPathingmgr.AddNode(23, new Dictionary<int, int>() { { 22, 20 }, { 17, 15 }, { 24, 10 } });
            ghostPathingmgr.AddNode(24, new Dictionary<int, int>() { { 23, 10 }, { 18, 15 } });
        }

        private void CreateEntities()
        {
            Entity newEntity;

            newEntity = new Fruit("fruit1", random.Next(15, 70));
            newEntity.AddComponent(new ComponentPosition(-22.5f, .5f, -22.5f));
            entityManager.AddEntity(newEntity);

            newEntity = new Fruit("fruit2", random.Next(15, 70));
            newEntity.AddComponent(new ComponentPosition(22.5f, .5f, -22.5f));
            entityManager.AddEntity(newEntity);

            newEntity = new Fruit("fruit3", random.Next(15, 70));
            newEntity.AddComponent(new ComponentPosition(-22.5f, .5f, 22.5f));
            entityManager.AddEntity(newEntity);

            newEntity = new Fruit("fruit4", random.Next(15, 70));
            newEntity.AddComponent(new ComponentPosition(22.5f, .5f, 22.5f));
            entityManager.AddEntity(newEntity);

            newEntity = new Fruit("fruit5", random.Next(15, 70));
            newEntity.AddComponent(new ComponentPosition(16.5f, .5f, -2.5f));
            entityManager.AddEntity(newEntity);

            LoadWalls();
            LoadPacDots();

            newEntity = new Player();
            newEntity.AddComponent(new ComponentPosition(0, 1.5f, -2.5f));
            newEntity.AddComponent(new ComponentAudio("eatfruit.wav"));
            newEntity.AddComponent(new ComponentAudio("eatghost.wav"));
            newEntity.AddComponent(new ComponentAudio("pickup.wav"));
            newEntity.AddComponent(new ComponentAudio("death.wav"));
            newEntity.AddComponent(new ComponentAudio("siren.wav"));
            newEntity.AddComponent(new ComponentAudio("willhelm.wav"));
            newEntity.AddComponent(new ComponentAudio("teleport.wav"));
            newEntity.AddComponent(new ComponentAudio("beginning.wav", false, false));
            newEntity.AddComponent(new ComponentCollision());
            entityManager.AddEntity(newEntity);


            newEntity = new Entity("skybox", EntityType.ENTITY_NONE);
            newEntity.AddComponent(new ComponentGeometry(new WrappedTexCube(new Vector3(60))));
            newEntity.AddComponent(new ComponentPosition(0, -15, 0));
            newEntity.AddComponent(new ComponentTexture("space_skybox.png"));
            entityManager.AddEntity(newEntity);


            newEntity = new Entity("floor", EntityType.ENTITY_NONE);
            newEntity.AddComponent(new ComponentPosition(0, -.2f, 0));
            newEntity.AddComponent(new ComponentGeometry(new Cube(new Vector3(50, 0.05f, 50))));
            newEntity.AddComponent(new ComponentTexture("floorLarge.png"));
            entityManager.AddEntity(newEntity);

            newEntity = new Ghost("pinkghost", "pink_ghost", pinkNodeptr, ghostPathingmgr);
            newEntity.AddComponent(new ComponentPosition(0, 2f, -10));
            respawner.AddGhostToQueue(newEntity.Name);
            entityManager.AddEntity(newEntity);

            newEntity = new Ghost("orangeghost", "orange_ghost", orangeNodePtr, ghostPathingmgr);
            newEntity.AddComponent(new ComponentPosition(0, 2f, -10));
            respawner.AddGhostToQueue(newEntity.Name);
            entityManager.AddEntity(newEntity);

            newEntity = new Ghost("blueghost", "blue_ghost", blueNodePtr, ghostPathingmgr);
            newEntity.AddComponent(new ComponentPosition(0, 2f, -10));
            respawner.AddGhostToQueue(newEntity.Name);
            entityManager.AddEntity(newEntity);

            newEntity = new Ghost("redghost", "red_ghost", redNodePtr, ghostPathingmgr);
            newEntity.AddComponent(new ComponentPosition(0, 2f, -10));
            respawner.AddGhostToQueue(newEntity.Name);
            entityManager.AddEntity(newEntity);

        }

        private void CreateSystems()
        {
            systemManager.AddSystem(new SystemRender("texture.vert", "texture.frag"));
            systemManager.AddSystem(new SystemPhysics());
            systemManager.AddSystem(new SystemAudio());
            systemManager.AddSystem(new SystemCollision());
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Update(FrameEventArgs e)
        {
            dt = (float)e.Time;
            TimeElapsed += dt;

            // required systems   
            var SystemCollision = (SystemCollision)systemManager.FindSystem("SystemCollision");

            var player = (Player)entityManager.FindEntity(EntityType.ENTITY_PLAYER);
            var other = entityManager.Entities.FindAll(s => s.Entity_Type != EntityType.ENTITY_PLAYER);


            if (DEBUG_COLLISION_ENABLED)
            {
                CollisionLoop(player); 
            }

            foreach (Entity entity in entityManager.Entities)
                entity.Update(dt);

            Respawner();
            GhostMovement();

            DeathDuty(player);

            systemManager.OnUpdateFrame(entityManager.Entities);
            UpdateModelViewProjections(); 
            camera.View = camera.GetViewMatrix(player.GetPosition);

            SystemAudio.SetListenerPosition(player.GetPosition.Position);

            // maintain the height of skybox so that draw distance is less of an issue
            var skybox = entityManager.FindEntity("skybox");
            if (skybox != null) skybox.OverrideComponent(new ComponentPosition(
                new Vector3(player.GetPosition.Position.X, skybox.GetPosition.Position.Y, player.GetPosition.Position.Z)));

            if (entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_MUNCH).Count == 0)
                GameEnd(player);
            if (player.IsDestroyed)
                GameEnd(player);
            if (sceneManager.Focused)
                UpdateMouseCursor();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="e">Provides a snapshot of timing values.</param>
        public override void Render(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
           
            GUI.clearColour = Color.CornflowerBlue;

            var renderer = (SystemRender)systemManager.FindSystem("SystemRender");

            systemManager.OnRenderFrame(entityManager.Entities, camera.View);


            // used to work but seems to have stopped displaying for no reason
            DrawUI();

        }

        private void UpdateModelViewProjections()
        {
            foreach(var entity in entityManager.Entities
                .Where(s => (s.Mask & ComponentTypes.COMPONENT_GEOMETRY) == ComponentTypes.COMPONENT_GEOMETRY))
            {
                var compGeo = (ComponentGeometry)entity.GetComponent(ComponentTypes.COMPONENT_GEOMETRY);

                compGeo._Object.ViewProjectionMatrix = camera.View *
                    Matrix4.CreatePerspectiveFieldOfView(1.3f, sceneManager.ClientSize.Width / (float)sceneManager.ClientSize.Height, 1.0f, 40f);
                compGeo._Object.ModelViewProjectionMatrix = compGeo._Object.ModelMatrix * compGeo._Object.ViewProjectionMatrix;
            }
        }

        private void DrawUI()
        {
            var player = (Player)entityManager.FindEntity(EntityType.ENTITY_PLAYER);
            string scoreboard = $"{player.Score}  {new string('❤', player.lives)}";

            float width = 250, fontSize = 20f;
            GUI.clearColour = Color.Black;
            GUI.Label(new Rectangle(25, (int)(fontSize / 2f), (int)width, (int)(fontSize * 2f)),
                scoreboard, (int)fontSize, StringAlignment.Center, Color.Yellow);
            GUI.Render();
        }

        private void Respawner()
        {
            string name = respawner.OnAction(ref TimeElapsed);

            if (name != null)
            {
                // spawn the ghost
                Ghost ghost = (Ghost)entityManager.Entities.Find(S => S.Name == name && S.Entity_Type == EntityType.ENTITY_GHOST);
                ghost.AIComponent.isActive = true;
            }

        }

        private void GhostMovement()
        {
            // happens in here instead of ghost because this requires less shimmying variables around
            var SystemCollision = (SystemCollision)systemManager.FindSystem("SystemCollision");
            var player = (Player)entityManager.FindEntity(EntityType.ENTITY_PLAYER);

            foreach (Ghost ghost in entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_GHOST))
            {
                if (ghost.AIComponent.isActive == false) continue;
                if (!DEBUG_GHOST_ENABLED)
                {
                    ghost.PathToNextNode(dt);
                    continue;
                }

                if (ghost.IsPlayerWithinVision(player.GetPosition.Position))
                {
                    var listOfWalls = entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_WALL);
                    if (SystemCollision.RayCastCollisionDetection(ghost.GetPosition.Position, player.GetPosition.Position,
                            listOfWalls) == false)
                    {
                        // player is within field of view and within line of sight

                        if (player.IsPoweredUp)
                        {
                            ghost.PathAwayFromPlayer(player.GetPosition.Position);
                        }
                        else
                        {
                            ghost.PathToPlayer(player.GetPosition.Position, dt); 
                        }
                    }
                    else
                    {
                        ghost.PathToNextNode(dt); 
                    }
                }
                else
                {
                    ghost.PathToNextNode(dt);
                }
            }
        }

        private void DeathDuty(Player player)
        {
            // on death function calls and entitylist cleanup
            foreach (var entity in entityManager.Entities.Where(s => s.IsDestroyed))
            {
                entity.OnDeath();

                if (entity.Entity_Type == EntityType.ENTITY_GHOST)
                    respawner.AddGhostToQueue(entity.Name);
                else if (entity.Entity_Type == EntityType.ENTITY_PLAYER)
                {
                    Close();
                    return;
                }
            }

            entityManager.Entities.RemoveAll(s => s.IsDestroyed);
        }

        private void CollisionLoop(Player player)
        {
            var other = entityManager.Entities.FindAll(s => s.Name != player.Name);
            var collision = (SystemCollision)systemManager.FindSystem("SystemCollision");

            string collideeName = string.Empty;
            EntityType collidee = EntityType.ENTITY_NONE;

            foreach (var entity in other.Where(s => (s.Mask & collision.MASK) == collision.MASK))
            {
                bool isCollision = collision.CalculateCollision(player, entity);

                if (isCollision)
                {
                    collidee = entity.Entity_Type;
                    collideeName = entity.Name;
                    break;
                }
            }

            if (collidee != EntityType.ENTITY_NONE)
            {
                HandleCollisionLogic(collidee, collideeName);
            }
        }

        private void HandleCollisionLogic(EntityType type, string Name)
        {
            var Player = (Player)entityManager.Entities.Find(s => s.Entity_Type == EntityType.ENTITY_PLAYER);

            // Call the appropiate event functions and set the collided entities to destroyed if applicable 
            switch (type)
            {
                case EntityType.ENTITY_WALL:
                    Player.RevertPlayerPosition();
                    break;
                case EntityType.ENTITY_PORTAL:
                    Player.Teleport(Name);
                    break;
                case EntityType.ENTITY_MUNCH:
                    Player.EatMunch();
                    entityManager.SetEntityDestroyed(Name);
                    break;
                case EntityType.ENTITY_FRUIT:
                    Player.PowerUp();
                    entityManager.SetEntityDestroyed(Name);
                    break;
                case EntityType.ENTITY_GHOST:
                    if (Player.IsPoweredUp)
                    {
                        entityManager.SetEntityDestroyed(Name);
                        Player.EatGhost();
                    }
                    else
                    {
                        if(DEBUG_GHOST_ENABLED)
                            PlayerLifeLost(Player);
                    }
                    break;
            }


        }

        private void PlayerLifeLost(Player player)
        {
            // call player life lost method
            // reset the respawn queues
            player.LifeLost();
            ResetGameWorld(player);
            respawner.Reset();

            foreach (var ghost in entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_GHOST))
            {
                respawner.AddGhostToQueue(ghost.Name);
            }
        }

        private void ResetGameWorld(Player player)
        {
            TimeElapsed = 0;

            foreach(Entity entity in entityManager.Entities.FindAll(s => s.Entity_Type == EntityType.ENTITY_GHOST))
            {
                Ghost ghost = (Ghost)entity;
                ghost.RespawnGhost();
                ghost.AIComponent.isActive = false;
            }

            camera.Rotation = new Vector3(MathHelper.DegreesToRadians(90), 0, 0); 
            player.ResetPlayerPosition();
        }

        private void GameEnd(Player player)
        {
            Close();
            sceneManager.StartGameOver(player.Score, player.lives);
        }
        /// <summary>
        /// This is called when the game exits.
        /// </summary>
        public override void Close()
        {
            sceneManager.KeyDown -= Keyboard_KeyPressed;
            sceneManager.Keyboard.KeyDown -= Keyboard_KeyDown;

            ResourceManager.CleanUpAudio();
            ResourceManager.CleanUpTextures();

            var systemAudio = (SystemAudio)systemManager.FindSystem("SystemAudio");
            systemAudio.Dispose();
        }

        private void Keyboard_KeyDown(object sender, KeyboardKeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    break;
                case Key.Down:
                    break;
                case Key.Space:
                    break;
                case Key.Escape:
                    this.Close();
                    this.sceneManager.StartMenu();
                    break;                    
            }
        }


        public void Keyboard_KeyPressed(object sender, KeyboardKeyEventArgs e)
        {
            var player = (Player)entityManager.Entities.Find(s => s.Entity_Type == EntityType.ENTITY_PLAYER);

            

            switch (e.Key)
            {
                case Key.W:
                case Key.Up:
                    player.Move(camera.Rotation, 0, 0.5f, 0);
                    break;
                case Key.A:
                case Key.Left:
                    player.Move(camera.Rotation, -0.5f, 0, 0);
                    break;
                case Key.S:
                case Key.Down:
                    player.Move(camera.Rotation, 0, -0.5f, 0);
                    break;
                case Key.D:
                case Key.Right:
                    player.Move(camera.Rotation, 0.5f, 0, 0);
                    break;
                // vertical
                case Key.Q:
                    if(!DEBUG_COLLISION_ENABLED) player.Move(camera.Rotation, 0, 0, 1f);
                    break;
                case Key.E:
                    if(!DEBUG_COLLISION_ENABLED) player.Move(camera.Rotation, 0, 0, -1f);
                    break;
                case Key.Number5:
                    player.ResetPlayerPosition();
                    break;
                case Key.C:
                    DEBUG_COLLISION_ENABLED = DEBUG_COLLISION_ENABLED == true ? false : true;
                    Console.WriteLine("Collision: " + DEBUG_COLLISION_ENABLED);
                    break;
                case Key.G:
                    DEBUG_GHOST_ENABLED = DEBUG_GHOST_ENABLED == true ? false : true;
                    Console.WriteLine("Ghost aggression: " + DEBUG_GHOST_ENABLED);
                    break;
            }
        }
    }
}
