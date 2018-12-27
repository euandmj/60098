using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Managers
{
    public class AIPathingManager
    {
        public readonly float[,] Nodes;
        private Graph Graph;

        public AIPathingManager(float[,] nodes)
        {
            // takes in a list of "vector3" nodes. 
            // Where nodes[x,z] = vec3(x, y, z)
            Nodes = nodes;
            Graph = new Graph(); 
        }

        public void AddNode(int name, Dictionary<int, int> edges)
        {
            Graph.AddNode(name, edges);
        }

        public Vector3 NthGlobalNode(int n, float height)
        {
            return new Vector3(Nodes[n, 0], height, Nodes[n, 1]);
        }

        public List<int> GetNodePath(int start, int end)
        {
            if (start == end)
                return new List<int> { start };

            return Graph.FindShortestPath(start, end);
        }

        public int FindNearestNodeTo(Vector3 position, float HEIGHT)
        {
            // closest index
            int index = 0;
            // smallest distance comparitor
            double distance = double.MaxValue;

            for (int i = 0; i < this.Nodes.GetLength(0); i++)
            {
                var nodePos = new Vector3(this.Nodes[i, 0], HEIGHT, this.Nodes[i, 1]);

                // ignore the sqrt for less performance hit
                double distTemp = Utility.Vec3Helper.DistanceBetween_Approx(position, nodePos);
                //double distTemp = Math.Pow(position.X - nodePos.X, 2) + Math.Pow(position.Y - nodePos.Y, 2) + Math.Pow(position.Z - nodePos.Z, 2);

                if (distTemp < distance)
                {
                    // overrite the shortest so far
                    index = i;
                    distance = distTemp;
                }
            }
            return index;
        }
    }

    // Constructs a graph out of vertices/nodes
    // Used for a Dijkstra smallest path algorithm
    internal class Graph
    {
        // list of all nodes within the graph and their connected nodes. 
        // <Node ID, Weight>
        Dictionary<int, Dictionary<int, int>> Nodes = new Dictionary<int, Dictionary<int, int>>();

        public void AddNode(int name, Dictionary<int, int> edges)
        {
            Nodes[name] = edges;
        }

        public List<int> FindShortestPath(int start, int end)
        {
            var previous = new Dictionary<int, int>();
            var distances = new Dictionary<int, int>();
            var nodes = new List<int>();

            List<int> path = new List<int>(0);

            foreach (var node in Nodes)
            {
                if (node.Key == start)
                {
                    distances[node.Key] = 0;
                }
                else
                {
                    distances[node.Key] = int.MaxValue;
                }

                nodes.Add(node.Key);
            }

            while (nodes.Count > 0)
            {
                // sort nodes by distance
                nodes.Sort((x, y) => distances[x] - distances[y]);

                var smallest = nodes[0];

                nodes.Remove(smallest);

                if (smallest == end)
                {
                    path = new List<int>();
                    // smallest viable path found. build back from target node
                    while (previous.ContainsKey(smallest))
                    {
                        path.Add(smallest);
                        smallest = previous[smallest];
                    }

                    break;
                }

                if (distances[smallest] == int.MaxValue)
                    break;

                foreach (var adjacent in Nodes[smallest])
                {
                    var temp = distances[smallest] + adjacent.Value;
                    if (temp < distances[adjacent.Key])
                    {
                        distances[adjacent.Key] = temp;
                        previous[adjacent.Key] = smallest;
                    }
                }
            }
            path.Reverse();
            return path;
        }
    }
}
