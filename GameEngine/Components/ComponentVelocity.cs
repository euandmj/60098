using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    public class ComponentVelocity : IComponent
    {
        public string Name { get; } = "ComponentVelocity";
        public ComponentVelocity(float x, float y, float z)
        {
            Velocity = new Vector3(x, y, z); 
        }

        public ComponentVelocity(Vector3 vel)
        {
            Velocity = vel;
        }

        public Vector3 Velocity { get; set; }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_VELOCITY; }
        }

        public void OnDestroy() { }
    }
}
