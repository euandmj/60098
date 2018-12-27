using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Entities;

namespace GameEngine.Systems
{
    public interface ISystem
    {
        void OnAction(Entity entity);

        // Property signatures: 
        string Name
        {
            get;
        }
    }
}
