using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Managers;
using System.Text;
using GameEngine.Utility;

namespace GameEngine.Components
{
    public class ComponentMaterial : IComponent
    {
        public string Name { get; internal set; } = "ComponentMaterial";
        public Material Material { get; set; }

        public ComponentMaterial(string path)
        {
            Material = ResourceManager.GetMaterial(path);
            Name = Material.Name;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_MATERIAL; }
        }

        public void OnDestroy() { }
    }
}
