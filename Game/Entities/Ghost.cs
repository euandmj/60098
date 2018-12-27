using GameEngine.Components;
using GameEngine.Managers;
using GameEngine.Entities;
using GameEngine.Systems;
using GameEngine.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using GameEngine.Geometry;
using System.Linq;

namespace OpenGL_Game.Entities
{
    public class Ghost : Entity
    {
        private const float VIEWRANGE = 25;
        private const float ANGLE_OF_VIEW = 0.77f;
        private const float moveSpeed = 5.5f;
        private const float HEIGHT = 1.5f;
        private const float WIDTH = 3f;
        private readonly ComponentTexture normalTexture, threatTexture;
        private AIPathingManager PathingManager;
        private Random rand;

        public Ghost(string name, string texture, int[] nodes, AIPathingManager mgr)
        {
            this.Name = name;
            Entity_Type = EntityType.ENTITY_GHOST;
            PathingManager = mgr;
            rand = new Random(); 
            // SpawnPoint = spawn;

            // AddComponent(new ComponentPosition(spawn));
            AddComponent(new ComponentGeometry(new WrappedTexCube(new Vector3(2.5f, 2.5f, 2.5f))));
            AddComponent(new ComponentAI(HEIGHT, nodes, mgr));
            AddComponent(new ComponentCollision());

            
            normalTexture = new ComponentTexture(texture + "_normal.png");
            threatTexture = new ComponentTexture(texture + "_threat.png");
            AddComponent(normalTexture); 
        }

        public void UpdatePosition(Vector3 newPos)
        {
            var pos = (ComponentPosition)GetComponent(ComponentTypes.COMPONENT_POSITION);
            pos.LastPos = GetPosition.Position;
            pos.Position = newPos;
        }

        public void RespawnGhost()
        {
            // resets the position to their first position after a player takes damage
            var newPos = PathingManager.NthGlobalNode(0, HEIGHT);
            // reset the texture 
            OverrideComponent(normalTexture); 
            var geoComponent = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            geoComponent._Object.Rotation = new Vector3(0, 0, 0); 
            AIComponent.masterCount = 0;
            UpdatePosition(newPos);
            AIComponent.RestartPath(PathingManager);
            AIComponent.isActive = false;
        }

        internal ComponentAI AIComponent
        {
            get
            {
                return (ComponentAI)GetComponent(ComponentTypes.COMPONENT_AI);
            }
        }

        public override void Update(float dt)
        {
            base.Update(dt);

        }

        public override void OnDeath()
        {
            // isDestroyed set true in systemcollision
            this.IsDestroyed = false;
            RespawnGhost();
        }

        public override float GetDepth()
        {
            return WIDTH;
        }

        public override float GetWidth()
        {
            return WIDTH;
        }

        private void FindNextNodeOnPath()
        {
            // according the ai component's path. return the index of the next node. 
            Vector3 targetNode = PathingManager.NthGlobalNode(AIComponent.masterCount, HEIGHT);

            int start = PathingManager.FindNearestNodeTo(GetPosition.Position, HEIGHT);
            int end = AIComponent.MasterNodePath[AIComponent.masterCount];

            List<int> path = PathingManager.GetNodePath(start, end);

            //AIComponent.SetPath(path);
        }                

        public void FaceTowards(Vector3 target, Vector3 last)
        {
            var geo = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            Vector3 targetVector = target - GetPosition.Position;
            Vector3 pos = GetPosition.Position;

            var dotproduct = Vector3.Dot((target - pos).Normalized(), GetPosition.DirectionalVector.Normalized());
            if (dotproduct == 1)
                return;
            else if(dotproduct == -1)
            {
                // ghost has turned back on itself
                geo._Object.Rotation += new Vector3(0, MathHelper.DegreesToRadians(180), 0);
                return; 
            }

            Vector3 worldVector = new Vector3(0, 0, -5);

            var angle = Vector3.CalculateAngle(worldVector, targetVector);
          //  geo._Object.Rotation = new Vector3(0, 0, 0); // reset rotation

            // fixed axis rotation
            if (target.X > pos.X)
                geo._Object.Rotation = Vector3.Zero - new Vector3(0, angle, 0); 
            else
                geo._Object.Rotation = Vector3.Zero + new Vector3(0, angle, 0);
        }

        public bool IsPlayerWithinVision(Vector3 playerPosition)
        {
            // checks the dot product between the entity and player vector's. 
            // if within a 45 FOV do stuff

            //Vector3 ghostVector = (nextNode - GetPosition.Position).Normalized();
            Vector3 ghostVector = (GetPosition.Position - GetPosition.LastPos).Normalized();
            Vector3 playerVector = (playerPosition - GetPosition.Position).Normalized();


            // make sure the player is within the viewrange. 
            var distance = Vec3Helper.DistanceBetween(GetPosition.Position, playerPosition);

            if (distance > VIEWRANGE)
                return false;


            var dotproduct = Vector3.Dot(ghostVector, playerVector);

            if (dotproduct > ANGLE_OF_VIEW)
            {
                //moveSpeed = 3f;

                return true;
            }
            return false;
        }

        public void PathAwayFromPlayer(Vector3 playerPosition)
        {
            // find a node that does not cross the player
            // if node vector dot player < view range
            // 

            // run a selection process on nodes
            // if the node is within dotproduct it is invalid
            // if the node is closer to the player than the ghost it is invalid
            List<int> validIndices = new List<int>(); 
            Vector3 pV = GetPosition.Position - playerPosition;

            // start at index 1 because spawn node doesnt count
            for(int i = 1; i < PathingManager.Nodes.GetLength(0); i++)
            {
                var node = PathingManager.NthGlobalNode(i, HEIGHT);
                Vector3 gV = node - GetPosition.Position;

                var dotproduct = Vector3.Dot(pV.Normalized(), gV.Normalized());

                if (dotproduct < ANGLE_OF_VIEW)
                {
                    // check the distance to node is less than player's
                    var playerDist = Vec3Helper.DistanceBetween_Approx(playerPosition, node);
                    var ghostDist = Vec3Helper.DistanceBetween_Approx(GetPosition.Position, node);
                    var ghostplayerdist = Vec3Helper.DistanceBetween_Approx(GetPosition.Position, playerPosition); 

                    if (playerDist > ghostDist && ghostDist > ghostplayerdist)
                        validIndices.Add(i);
                }

            }
            // unsure of how to further select the best node to go to. 
            if (validIndices.Count == 0)
            {
                Console.WriteLine("Error in finding a valid node to run to"); 
            }
            // randomly select a valid node
            var selectednode = validIndices[rand.Next(0, validIndices.Count)];

            // create the new path along this node route
            var newPath = PathingManager.GetNodePath(PathingManager.FindNearestNodeTo(GetPosition.Position, HEIGHT), selectednode);

            // Check if the closest node should be pathed to. 
            // newPath is created between the nearest node and target and so does not necessarily include the closest node. 
            var closestNode = PathingManager.FindNearestNodeTo(GetPosition.Position, HEIGHT);
            var player_distance_to_closest_node = Vec3Helper.DistanceBetween_Approx(playerPosition, PathingManager.NthGlobalNode(closestNode, HEIGHT));
            var ghost_distance_to_closest_node = Vec3Helper.DistanceBetween_Approx(GetPosition.Position, PathingManager.NthGlobalNode(closestNode, HEIGHT));

            if(ghost_distance_to_closest_node < player_distance_to_closest_node)
                newPath.Insert(0, PathingManager.FindNearestNodeTo(GetPosition.Position, HEIGHT));

            // remove duplicates in the case newPath includes the closest pathh. 
            var path = newPath.Distinct().ToList(); 

            AIComponent.RestartPath(path);

            // small hack to get the directional vector to reset so that the ghost moves along new path next frame
            GetPosition.LastPos = new Vector3(0);  



        }

        public void PathToPlayer(Vector3 playerPosition, float dt)
        {
            OverrideComponent(threatTexture);
            Vector3 directionalVector = Vec3Helper.MoveTowards(GetPosition.Position, playerPosition, moveSpeed * dt);
            //FaceTowards(playerPosition); 
            FaceTowards(playerPosition, PathingManager.NthGlobalNode(AIComponent.GetLastNode(), HEIGHT)); 
            UpdatePosition(directionalVector); 
        }

        public void PathToNode(int node, float dt)
        {
            // vector3 position of the closest node to the player 
            Vector3 targetPos = PathingManager.NthGlobalNode(node, HEIGHT);

            Vector3 directionalVector = Vec3Helper.MoveTowards(GetPosition.Position, targetPos, moveSpeed * dt);

            UpdatePosition(directionalVector); 
            
        }

        public void PathToNextNode(float dt)
        {
            OverrideComponent(normalTexture);

            var lastnode = PathingManager.NthGlobalNode(AIComponent.GetLastNode(), HEIGHT);

            int nodeptr = AIComponent.GetNextNode();

            Vector3 targetNode = PathingManager.NthGlobalNode(nodeptr, HEIGHT);

            if (GetPosition.Position == targetNode)
            {
                AIComponent.ShiftTempPointer(PathingManager);
                // rotate the ghost to face this new direction
                targetNode = PathingManager.NthGlobalNode(AIComponent.GetNextNode(), HEIGHT);
                FaceTowards(targetNode, lastnode);
                //DebugConsoleReport();
            }



            Vector3 directionalVector = Vec3Helper.MoveTowards(GetPosition.Position, targetNode, moveSpeed * dt);
            
            UpdatePosition(directionalVector);
        }


        private void DebugConsoleReport()
        {
            if (Name.Contains("pink"))
                Console.ForegroundColor = ConsoleColor.Magenta;
            else if (Name.Contains("orange"))
                Console.ForegroundColor = ConsoleColor.DarkRed;
            else if (Name.Contains("blue"))
                Console.ForegroundColor = ConsoleColor.Cyan;
            else if (Name.Contains("red"))
                Console.ForegroundColor = ConsoleColor.Red;


            Console.WriteLine($"{this.Name} is now pathing to node {AIComponent.GetNextNode()}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
