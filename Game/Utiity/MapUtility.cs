using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GameEngine.Entities;
using OpenTK;
using GameEngine.Components;
using GameEngine.Geometry;
using OpenGL_Game.Entities;

namespace GameEngine.Utility
{
    static class MapUtility
    {      
        internal struct NodeCoordinate
        {
            public float X;
            public float Z;
        }

        public static Entity LoadWall(float x, float y, float z, float w, float h, float d, string texture, int count)
        {
            string entityName = $"wall_{x},{z}_";
            Entity wall = new Entity(entityName + count.ToString(), EntityType.ENTITY_WALL);
            wall.AddComponent(new ComponentTexture(texture));
            wall.AddComponent(new ComponentCollision());
            wall.AddComponent(new ComponentPosition(x,y,z));
            wall.AddComponent(new ComponentGeometry(new Cube(new Vector3(w,h,d))));

            return wall;
        }

        public static Entity LoadWallPortal(float x, float y, float z, float w, float h, float d, string texture, string name)
        {
            Entity wall = new Entity(name, EntityType.ENTITY_PORTAL);
             
            wall.AddComponent(new ComponentTexture(texture));
            wall.AddComponent(new ComponentCollision());
            wall.AddComponent(new ComponentPosition(x, y, z)); 
            wall.AddComponent(new ComponentGeometry(new Cube(new Vector3(w, h, d))));
            
            return wall;
        }

        public static Entity LoadGhostSpawnWall(float x, float y, float z, float w, float h, float d, string texture, string name)
        {
            Entity wall = new Entity(name, EntityType.ENTITY_WALL);

            wall.AddComponent(new ComponentTexture(texture, true));
            wall.AddComponent(new ComponentCollision());
            wall.AddComponent(new ComponentPosition(x, y, z));
            wall.AddComponent(new ComponentGeometry(new Cube(new Vector3(w, h, d))));

            return wall;
        }

        public static Pacdot LoadPacdot(float x, float y, float z, int count)
        {
            Pacdot pacdot = new Pacdot($"pacdot_{x},{z}_" + count.ToString(), new Vector3(0));

            pacdot.AddComponent(new ComponentPosition(new Vector3(x, y, z)));
            return pacdot;
        }

        public static float[,] NodeLoader (string path)
        {
            List<NodeCoordinate> list = new List<NodeCoordinate>();
            string file = string.Empty;
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    file = sr.ReadToEnd();
                }
            }
            catch(FileNotFoundException)
            {
                Console.WriteLine("Error reading in node file"); 
            }
            string[] lines = file.Split('\n');

            try
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith("#") || line.Length < 1) continue;

                    string[] parts = line.Trim().Split(',');

                    bool isParseOk;
                    isParseOk = float.TryParse(parts[0], out float x);
                    isParseOk &= float.TryParse(parts[1], out float z);

                    if (!isParseOk)
                        throw new Exception("Error parsing coordinates of line: " + line);

                    var coord = new NodeCoordinate
                    {
                        X = x,
                        Z = z
                    };

                    list.Add(coord);

                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // parse into array
            // bit of a hack

            float[,] array = new float[list.Count, 2];

            for(int i = 0; i < list.Count; i++)
            {
                array[i, 0] = list[i].X;
                array[i, 1] = list[i].Z;
            }

            return array;
        }

        
    }
}
