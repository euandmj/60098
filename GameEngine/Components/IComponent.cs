using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameEngine.Components
{
    [Flags]
    public enum ComponentTypes {
        COMPONENT_NONE      = 0,
	    COMPONENT_POSITION  = 1 << 0,
        COMPONENT_GEOMETRY  = 1 << 1,
        COMPONENT_TEXTURE   = 1 << 2,
        COMPONENT_MATERIAL  = 1 << 3,
        COMPONENT_VELOCITY  = 1 << 4,
        COMPONENT_AUDIO     = 1 << 5,
        COMPONENT_COLLISION = 1 << 6,
        COMPONENT_AI  = 1 << 7
    }  

    public interface IComponent
    {
        ComponentTypes ComponentType
        {
            get;
        }
        string Name { get; }

        void OnDestroy(); 
    }
}
