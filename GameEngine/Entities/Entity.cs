using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GameEngine.Components;

namespace GameEngine.Entities
{
    public enum EntityType
    {
        ENTITY_NONE,
        ENTITY_MUNCH,
        ENTITY_PLAYER,
        ENTITY_GHOST,
        ENTITY_WALL,
        ENTITY_FRUIT,
        ENTITY_PORTAL,
    }

    public class Entity
    {
        public bool IsDestroyed { get; set; } = false;
        public ComponentTypes Mask { get; set; }
        public virtual string Name { get; set; }
        public EntityType Entity_Type { get; set; }

        List<IComponent> componentList = new List<IComponent>();


        public Entity() { }

        public Entity(string name)
        {
            Name = name;
        }
 
        public Entity(string name, EntityType type)
        {
            Name = name;
            Entity_Type = type;
        }

        public virtual float GetWidth()
        {
            var geometry = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            return geometry._Object.Width;
        }

        public virtual float GetRadius()
        {
            var geometry = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            return geometry._Object.Radius;
        }

        public virtual float GetDepth()
        {

            var geometry = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            return geometry._Object.Depth;
        }

        /// <summary>Adds a single component</summary>
        public void AddComponent(IComponent component)
        {
            Debug.Assert(component != null, "Component cannot be null");

            componentList.Add(component);
            Mask |= component.ComponentType;
        }
        

        public IComponent GetComponent(ComponentTypes type)
        {
            return componentList.FirstOrDefault(s => type == s.ComponentType);
        }

        public IComponent GetComponent(ComponentTypes type, string name)
        {
            return componentList.FirstOrDefault(s => type == s.ComponentType && s.Name == name);
        }

        /* Specific Component Gets */

        public ComponentAudio GetComponentAudio(string name)
        {            
            return componentList.FirstOrDefault(s => ComponenTypes.ComponentAudio == s.ComponentType && s.Name == name);            
        }

        public ComponentCollision GetComponentCollision(string name)
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentCollision == s.ComponentType && s.Name == name);
        }

        public ComponentGeometry GetComponentGeometry(string name)
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentGeometry == s.ComponentType && s.Name == name);            
        }

        public ComponentMaterial GetComponentMaterial(string name)
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentMaterial == s.ComponentType && s.Name == name);
        }

        public ComponentVelocity GetComponentVelocity()
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentVelocity == s.ComponentType);            
        }
        
        public ComponentPosition GetComponentPosition()
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentPosition == s.ComponentType);
        }

        public ComponentTexture GetComponentTexture(string name)
        {
            return componentList.FirstOrDefault(s => ComponentTypes.ComponentTexture == s.ComponentType && s.Name == name);
        }

        public ComponentTexture GetComponentTexture(int texid)
        {
            foreach(ComponentTexture tex in componentList)
            {
                if(tex.Texture == texid)
                    return tex;
            }
            return null;
        }

        public void OverrideComponent(IComponent newComponent)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                if (componentList[i].ComponentType == newComponent.ComponentType)
                {
                    componentList[i] = newComponent;
                    return;
                }
            }
        }

        public void OverrideComponent(string name, IComponent newComponent)
        {
            for (int i = 0; i < componentList.Count; i++)
            {
                if (componentList[i].ComponentType == newComponent.ComponentType && componentList[i].Name == name)
                {
                    componentList[i] = newComponent;
                    return;
                }
            }
        }

        public ComponentPosition GetPosition
        {
            get
            {
                return (ComponentPosition)GetComponent(ComponentTypes.COMPONENT_POSITION);
            }
        }



        public List<IComponent> GetComponents(ComponentTypes type)
        {
            return componentList.FindAll(s => s.ComponentType == type);
        }

        public virtual void Update(float dt)
        {

        }

        public virtual void OnDeath()
        {
            foreach(var component in componentList)
            {
                component.OnDestroy(); 
            }
        }
    }
}
