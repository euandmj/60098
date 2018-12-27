using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using GameEngine.Components;
using System.IO;
using GameEngine.Entities;

namespace GameEngine.Systems
{
    public class SystemPhysics : ISystem
    {
        // add time interval
        const ComponentTypes MASK = ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_VELOCITY;

        public SystemPhysics()
        {            
        }

        public string Name
        {
            get { return "SystemPhysics"; }
        }

        public void OnAction(Entity entity)
        {
            if ((entity.Mask & MASK) == MASK)
            {
                var componentVelocity = (ComponentVelocity)entity.GetComponent(ComponentTypes.COMPONENT_VELOCITY);
                var componentPosition = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION); 
                

                Motion(componentPosition, componentVelocity.Velocity);
            }
        }

        public void Motion(ComponentPosition pos, Vector3 Velocity)
        {
            pos.LastPos = pos.Position;
            pos.Position += Velocity;
        }       

    }
}
