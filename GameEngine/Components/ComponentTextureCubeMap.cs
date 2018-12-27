using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Managers;
using System.Text;

namespace GameEngine.Components
{
    class ComponentTextureCubeMap : IComponent
    {
        public readonly OpenTK.Graphics.OpenGL.TextureTarget TextureTarget;

        public ComponentTextureCubeMap(string[] textureNames, OpenTK.Graphics.OpenGL.TextureTarget texTarget)
        {
            Texture = ResourceManager.LoadTextureCubeMap(textureNames); 
            TextureTarget = texTarget;
        }

        public int Texture { get; }

        public string Name { get { return "ComponentTextureCubeMap"; } }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }

        public void OnDestroy()
        {
        }
    }
}
