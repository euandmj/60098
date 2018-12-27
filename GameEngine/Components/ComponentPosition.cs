using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;

namespace GameEngine.Components
{
    public class ComponentPosition : IComponent
    {
        public string Name { get; } ="ComponentPosition"; 
        public ComponentPosition(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        public ComponentPosition(Vector3 pos)
        {
            Position = pos;
        }

        public Vector3 DirectionalVector
        {
            get { return Position - LastPos; }
        }

        public Vector3 Position { get; set; }

        public Vector3 LastPos { get; set; }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_POSITION; }
        }

        public void RevertPosition()
        {
            // swaps the entity's position back to the last known position.
            this.Position = LastPos;
        }

        public void OnDestroy() { }

        
    }
}
