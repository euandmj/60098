using GameEngine.Components;
using GameEngine.Entities;
using GameEngine.Systems;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GameEngine.Managers
{
    public class EntityManager
    {
        //List<Entity> entityList;

        public EntityManager()
        {
            Entities = new List<Entity>(); 
        }

        public void AddEntity(Entity entity)
        {
            Entities.Add(entity);
        }

        public void AddEntities(List<Entity> entities)
        {
            Entities.AddRange(entities);
        }

        public Entity FindEntity(string name)
        {
            return Entities.Find(delegate (Entity e)
            {
                return e.Name == name;
            });
        }        

        public Entity FindEntity(EntityType type)
        {
            return Entities.FirstOrDefault(s => s.Entity_Type == type);
        }

        public List<Entity> Entities { get; set; }

        public void SetEntityDestroyed(string entityName)
        {
            var target = Entities.Find(s => s.Name == entityName);

            if (target == null)
                throw new ArgumentException("This entity "+ entityName + " does not exist");

            target.IsDestroyed = true;
        }

        public void SetEntityDestroyed(string entityName, EntityType type)
        {
            var target = Entities.Find(s => s.Name == entityName && s.Entity_Type == type);

            if (target == null)
                throw new ArgumentException("This entity " + entityName + " does not exist");

            target.IsDestroyed = true;
        }

        public void SetEntitiesDestroyed(EntityType type)
        {
            foreach (var entity in Entities.Where(s => s.Entity_Type == type))
                entity.IsDestroyed = true;
        }

        public void SetEntitiesDestroyed(EntityType type, int n)
        {
            foreach (var entity in Entities.Where(s => s.Entity_Type == type).Take(n))
                entity.IsDestroyed = true;
        }
    }
}
