using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using OpenTK;
using OpenTK.Graphics;
using GameEngine.Entities;

namespace GameEngine.Utility
{
    /// <summary>
    /// Class used for loading 3d geometry text files.
    /// These files have been formatted in a similar way to .objs loaded by ModelUtility.cs
    /// </summary>
    
    /// <summary>
    /// Load files into Shape objects
    /// </summary>
    class PrimativeTypeUtility
    {
        public static void LoadGeometry(string path)
        {
            string fileContents = string.Empty;
            string[] lines;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    fileContents = sr.ReadToEnd(); 
                }
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine($"{ex.Message}: \n{path}");
            }


            lines = fileContents.Split('\n');

            var _vertices = new List<Vector3>();
            var _colors = new List<Vector3>();
            var _indices = new List<int>();
            var _texutes = new List<Vector2>();

            foreach(string line in lines)
            {
                if (line.StartsWith("#") || line == "") continue;

                if (line.StartsWith("v"))
                {
                    // parse vertices
                    _vertices.Add(ParseVector3(line.Substring(1).Trim()));
                }
                else if (line.StartsWith("i"))
                {

                }
            }

        }

        private static Vector3 ParseVector3(string line)
        {
            var vec = new Vector3();
            bool isParseOk;

            string[] data = line.Trim().Split(' ');

            isParseOk = float.TryParse(data[0], out vec.X);
            isParseOk &= float.TryParse(data[1], out vec.X);
            isParseOk &= float.TryParse(data[2], out vec.Z);

            if(!isParseOk)
                Console.WriteLine($"Error parsing a texture line. Line: {line}");

            return vec;
        }

        private static Vector2 ParseVector2(string line)
        {
            var vec = new Vector2();
            bool isParseOk;

            string[] data = line.Trim().Split(' ');

            isParseOk = float.TryParse(data[0], out vec.X);
            isParseOk &= float.TryParse(data[1], out vec.X);

            if (!isParseOk)
                Console.WriteLine($"Error parsing a vector3. Line: {line}");

            return vec;
        }

        private static List<int> ParseIndices(string line)
        {
            List<int> inds = new List<int>();
            string[] indsRaw = null;
            bool isParseOk;

            // split the line depending if it indices have been formatted with commas or spaces
            if (line.Count((char c) => c == ',') > 0)
                indsRaw = line.Split(',');
            else if (line.Count((char c) => c == ' ') > 0)
                indsRaw = line.Split(' ');

            foreach(string ind in indsRaw)
            {
                isParseOk = int.TryParse(ind, out int indice);

                if(!isParseOk)
                    Console.WriteLine($"Error parsing an indice. Line: {line}");

                inds.Add(indice);
            }

            return inds;
        }
    }
}
