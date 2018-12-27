using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK.Graphics;
using GameEngine.Managers;
using OpenTK.Graphics.OpenGL;

namespace GameEngine.Components
{
    public class ComponentTexture : IComponent
    {
        public string Name { get; internal set; } = "ComponentTexture";
        public readonly TextureTarget TextureTarget;

        public ComponentTexture(string textureName)
        {
            Texture = ResourceManager.LoadTexture2D(textureName);
            Name = textureName;
            TextureTarget = TextureTarget.Texture2D;
        }

        public ComponentTexture(string textureName, TextureTarget textarget)
        {
            // Call the correct load texture
            // make a generic texture load function?
            Texture = ResourceManager.LoadTexture2D(textureName);
            TextureTarget = textarget;
            Name = textureName;
        }

        public ComponentTexture(string[] textureNames)
        {
            Texture = ResourceManager.LoadTextureCubeMap(textureNames);
            TextureTarget = TextureTarget.TextureCubeMap;
        }

        public ComponentTexture(string textureName, bool alpha)
        {
            Texture = ResourceManager.LoadTextureAlphaOnly(textureName);
            TextureTarget = TextureTarget.Texture2D;
        }


        public int Texture { get; }

        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_TEXTURE; }
        }

        public void OnDestroy()
        {
        }
    }
}
