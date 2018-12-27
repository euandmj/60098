using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameEngine.Managers;
using GameEngine.Entities;
using GameEngine.Geometry;

namespace GameEngine.Components
{
    public class ComponentGeometry : IComponent
    {
        public string Name { get; internal set; } = "ComponentGeometry";
        public Shape _Object { get; set; }

        public ComponentGeometry()
        {
        }

        public ComponentGeometry(string name)
        {
            _Object = ResourceManager.GetModel("Assets/Models/" + name); 
        }

        public ComponentGeometry(string name, float scale)
        {
            _Object = ResourceManager.GetModel("Assets/Models/" + name);
            _Object.Scale = new OpenTK.Vector3(scale); 
        }

        public ComponentGeometry(string name, OpenTK.Vector3 scale)
        {
            _Object = ResourceManager.GetModel(name);
            _Object.Scale = scale;
        }

        public ComponentGeometry(Shape shape)
        {
            _Object = shape;
        }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_GEOMETRY; }
        }


        public void OnDestroy() { }
    }
}
