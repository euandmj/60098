using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GameEngine.Managers;
using System.Xml.XPath;
using OpenTK;

namespace GameEngine.Components
{
    public class ComponentAI : IComponent
    {
        private readonly float HEIGHT;
        public string Name { get; } = "ComponentAI";
        public bool isActive = false;


        public int[] MasterNodePath { get; set; }
        public List<int> TempNodePath { get; set; }

        public int masterCount = 0;
        public int tempCount = 0;


        public int GetNextNode()
        {
            return TempNodePath[tempCount];
        }

        public int GetLastNode()
        {
            var count = masterCount == 0 ? MasterNodePath.Length - 1 : masterCount - 1;
            return count;
        }

        public ComponentAI(float height, int[] nodeptrs, AIPathingManager mgr)
        {
            HEIGHT = height; 
            MasterNodePath = nodeptrs;

            var start = 0;
            var end = MasterNodePath[masterCount];

            TempNodePath = mgr.GetNodePath(start, end);

        }

        public ComponentAI(float height, string filePath)
        {
            HEIGHT = height;

        }

        public void ShiftTempPointer(AIPathingManager mgr)
        {
            if(tempCount == TempNodePath.Count - 1)
            {
                // manual increment and temporary path reset
                // create a new path between the next set of nodes
                var start = TempNodePath[tempCount];
                tempCount = 0;
                ShiftMasterPointer();

  
                var end = MasterNodePath[masterCount];

                var path = mgr.GetNodePath(start, end);
                TempNodePath = path;
            }
            else
                tempCount++;
        }

        public void RestartPath(AIPathingManager mgr)
        {
            // This function is called in the state of a ghost dying. 
            // to prevent clipping through walls, the path needs to be recreated. 
            tempCount = 0;
            var end = MasterNodePath[masterCount];

            var path = mgr.GetNodePath(0, end);
            TempNodePath = path;
        }

        public void RestartPath(AIPathingManager mgr, Vector3 pos)
        {
            // this overload jumps back onto the predetermined node path
            var start = mgr.FindNearestNodeTo(pos, HEIGHT);
            tempCount = 0;
            var end = MasterNodePath[masterCount];

            var path = mgr.GetNodePath(start, end);
            TempNodePath = path;           
        }

        public void RestartPath(List<int> path)
        {
            TempNodePath = path;
            tempCount = 0;
        }

        public void ShiftMasterPointer()
        {
            // Shift node pointer. Circular linked lists are not supported
            masterCount = masterCount == MasterNodePath.Length - 1 ? 0 : masterCount + 1;

        }

        public void ShiftMasterPointerBackwds()
        {
            masterCount = masterCount == 0 ? MasterNodePath.Length - 1 : masterCount - 1; 
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AI; }
        }

        public void OnDestroy() { }

    }
}
