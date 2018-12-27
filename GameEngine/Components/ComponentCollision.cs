using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using System.Text;


namespace GameEngine.Components
{
    public class ComponentCollision : IComponent
    {
        public string Name { get; } = "ComponentCollision";

        public ComponentCollision()
        {

        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_COLLISION; }
        }

        public void OnDestroy() { }
    }
}
