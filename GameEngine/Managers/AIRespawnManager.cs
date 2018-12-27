using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Entities;

namespace GameEngine.Managers
{
    internal struct QueueItem
    {
        public string Name;
        public bool isActive; 

        public QueueItem(string name)
        {
            Name = name;
            isActive = false;
        }
    }

    public class RespawnManager
    {
        private readonly float RespawnTime;
        private Queue<string> Queue;

        public RespawnManager(float respawnTime)
        {
            RespawnTime = respawnTime;
            Queue = new Queue<string>();
        }

        public void Reset()
        {
            Queue.Clear();
        }

        public void AddGhostToQueue(string name)
        {
            Queue.Enqueue(name);
        }

        public string GetNextInLineGhost()
        {
            return Queue.Count > 0 ? Queue.First() : null;
        }

        public string OnAction(ref float timeElapsed)
        {
            // Called once per update. checks if time elapsed is greater than the respawn timer and
            // if so return true

            if(timeElapsed > RespawnTime)
            {
                timeElapsed = 0; 
                if(Queue.Count > 0)
                {
                    return Queue.Dequeue();
                }
            }
            return null;
        }       
    }
}
