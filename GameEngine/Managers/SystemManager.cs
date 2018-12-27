using GameEngine.Entities;
using GameEngine.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using GameEngine.Components;

namespace GameEngine.Managers
{
    public class SystemManager
    {
        List<ISystem> systemList = new List<ISystem>();

        public SystemManager()
        {
        }

        public void OnUpdateFrame(List<Entity> entityList)
        {
            foreach(ISystem system in systemList)
            {               
                foreach(Entity entity in entityList)
                {
                    system.OnAction(entity);
                }
            }
        }

        public void OnRenderFrame(List<Entity> Entities, OpenTK.Matrix4 cameraView)
        {
            var sysRender = (SystemRender)FindSystem("SystemRender");

            if (sysRender == null) return;

            foreach(var entity in Entities)
            {
                sysRender.RenderFrame(entity, cameraView);
            }
        }

        public void AddSystem(ISystem system)
        {
            ISystem result = FindSystem(system.Name);
            //Debug.Assert(result != null, "System '" + system.Name + "' already exists");
            systemList.Add(system);
        }

        public ISystem FindSystem(string name)
        {
            return systemList.Find(delegate(ISystem system)
            {
                return system.Name == name;
            });
        }
    }
}
