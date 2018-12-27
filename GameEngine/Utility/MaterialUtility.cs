using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameEngine.Utility
{
    class MaterialUtility
    {
        // generic material of RGB component.
        public static readonly Material GenericMaterial = new Material(new Vector3(0.15f), new Vector3(1), new Vector3(0.2f), 5);

        public static List<Material> LoadMaterial(string path)
        {
            List<Material> temp = new List<Material>();
            string mat = "";
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string line;

                    while (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();

                        if (!line.StartsWith("newmtl"))
                        {
                            if (mat.StartsWith("newmtl"))
                            {
                                mat += line + '\n';
                            }
                        }
                        else
                        {
                            if (mat.Length > 0)
                            {
                                Material newMaterial = new Material();

                                newMaterial = ReadMaterial(mat, out newMaterial.Name);
                                temp.Add(newMaterial); 
                            }
                            mat = line + '\n';
                        }
                    }
                }
                if (mat.Count((char c) => c == '\n') > 0)
                {
                    Material newMaterial = new Material();

                    newMaterial = ReadMaterial(mat, out newMaterial.Name);
                    temp.Add(newMaterial);
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Error locating file: {path}");
            }
            return temp;
        }

        private static Material ReadMaterial(string matfile, out string matname)
        {
            Material temp = new Material();
            const string newmaterialidentifier = "newmtl ";
            matname = "";
            string[] split = matfile.Split('\n');
            split.SkipWhile(s => !s.StartsWith(newmaterialidentifier));

            if (split.Length > 0)
            {
                matname = split[0].Substring(newmaterialidentifier.Length);
            }

            // trims every element
            split = split.Select((string s) => s.Trim()).ToArray();

            // parses the material, similar to obj files
            foreach (string line in split)
            {
                // line is not about material data
                if (line.Length < 3 || line.StartsWith("//") || line.StartsWith("#")) continue;

                if (line.StartsWith("Kd"))
                    temp.DiffuseColor = ParseVector3(line);
                else if (line.StartsWith("Ka"))
                    temp.AmbientColor = ParseVector3(line);
                else if (line.StartsWith("Ks"))
                    temp.SpecularColor = ParseVector3(line);

                else if (line.StartsWith("Ns"))
                    temp.SpecularExponent = ParseSpecularMod(line);

                else if (line.StartsWith("map_Ka"))
                    temp.AmbientMap = ParseMapData("map_Ka", line);
                else if (line.StartsWith("map_Kd"))
                    temp.DiffuseMap = ParseMapData("map_Kd", line);
                else if (line.StartsWith("map_Ks"))
                    temp.SpecularMap = ParseMapData("map_Ks", line);

                else if (line.StartsWith("map_normal"))
                    temp.NormalMap = ParseMapData("map_normal", line);
                else if (line.StartsWith("map_opacity"))
                    temp.OpacityMap = ParseMapData("map_opacity", line);
            }
            return temp;
        }

        private static Vector3 ParseVector3(string line)
        {
            Vector3 vector3 = new Vector3();
            string[] data = line.Substring(3).Split(' ');
            bool isParseSuccess;
            // error handling
            if (data.Length < 3)
            {
                throw new ArgumentException($"Error parsing data at:\n{line}");
            }

            isParseSuccess = float.TryParse(data[0], out vector3.X);
            isParseSuccess |= float.TryParse(data[1], out vector3.Y);
            isParseSuccess |= float.TryParse(data[2], out vector3.Z);

            if (!isParseSuccess)
            {
                Console.WriteLine($"Error reading data at:\n{line}");
            }

            return vector3;
        }

        private static float ParseSpecularMod(string line)
        {
            bool isParseSuccess = float.TryParse(line.Substring(3), out float result);
            if (!isParseSuccess)
            {
                Console.WriteLine($"Error parsing specular modifier data at:\n{line}");
            }

            return result;
        }

        private static string ParseMapData(string mapType, string line)
        {
            return line.Length > mapType.Length + 6 ? line.Substring(mapType.Length + 1) : $"{mapType}_ERROR:{line}";
        }
    }
}
