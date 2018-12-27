using GameEngine.Components;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using GameEngine.Entities;

namespace GameEngine.Systems
{
    public class SystemCollision : ISystem
    {
        public readonly ComponentTypes MASK = ComponentTypes.COMPONENT_COLLISION | ComponentTypes.COMPONENT_POSITION | ComponentTypes.COMPONENT_GEOMETRY;
        public string Name { get { return "SystemCollision"; } }

        public void OnAction(Entity e)
        {

        }

        public bool CalculateCollisionWithCircle(Entity entity1, Entity entity2)
        {
            Vector3 pos1 = entity1.GetPosition.Position;
            Vector3 pos2 = entity2.GetPosition.Position;
            float width1 = entity1.GetWidth();
            float width2 = entity2.GetWidth();
            float radius2 = entity2.GetRadius();

            float dx = Math.Abs(pos2.X - pos1.X - width1 / 2);
            float dz = Math.Abs(pos2.Z - pos1.Z - width1 / 2);

            if (dx > (width1 / 2 + radius2) || dz > (width1 / 2 + radius2))
                return false;

            if (dx <= width1 / 2 || dz <= width1 / 2)
                return true;

            var ddx = dx - width1 / 2;
            var ddz = dz - width1 / 2;

            // If ddx^2 + ddy^2 is less than or equal to radius squared then collision is true.
            return ddx * ddx + ddz * ddz <= radius2 * radius2;
        }

        public bool CalculateCollision(Vector3 location, Entity entity)
        {
            Vector3 pos2 = entity.GetPosition.Position;
            float d = entity.GetDepth() / 2;
            float w = entity.GetWidth() / 2;

            bool xColl = location.X >= pos2.X - w && location.X <= pos2.X + w;
            bool zColl = location.Z >= pos2.Z - d && location.Z <= pos2.Z + d;

            return xColl & zColl;
        }

        public bool CalculateCollision(Entity entity1, Entity entity2)
        {
            Vector3 pos1 = entity1.GetPosition.Position;
            Vector3 pos2 = entity2.GetPosition.Position;

            float w1 = entity1.GetWidth() / 2;
            float w2 = entity2.GetWidth() / 2;
            float d1 = entity1.GetDepth() / 2;
            float d2 = entity2.GetDepth() / 2;

            bool xColl = pos1.X + w1 >= pos2.X - w2 && pos1.X - w1 <= pos2.X + w2;
            bool zColl = pos1.Z + d1 >= pos2.Z - d2 && pos1.Z - d1 <= pos2.Z + d2;

            return xColl && zColl;
        }

        public bool RayCastCollisionDetection(Vector3 pos, Vector3 targetPos, List<Entity> entities)
        {
            // logic seems to be correct but causes complete thread lockup 

            // draw a ray from A to B. if any point on this ray causes a collision, return true
            // get a line between two points 

            var distanceBetween = Utility.Vec3Helper.DistanceBetween_Approx(pos, targetPos);
            List<Entity> validEntities = new List<Entity>();

            // trim entities down to only those near

            for(int i = 0; i < entities.Count; i++)
            {
                var distTemp = Utility.Vec3Helper.DistanceBetween_Approx(pos, entities[i].GetPosition.Position);
                if (distTemp >= distanceBetween)
                    entities.RemoveAt(i); 
            }
            //foreach (Entity e in entities)
            //{
            //    var distTemp = Utility.Vec3Helper.DistanceBetween_Approx(pos, e.GetPosition.Position);
            //    if (distTemp <= distanceBetween)
            //        validEntities.Add(e);
            //}

            var ray = Utility.Vec3Helper.MoveTowards(pos, targetPos, 1f);

            bool isColl = false;
            while (ray != targetPos)
            {
                ray = Utility.Vec3Helper.MoveTowards(ray, targetPos, 1f);

                Parallel.ForEach(entities, (wall) =>
                {
                    isColl |= CalculateCollision(ray, wall);
                    
                }); 
            }


            return isColl;
        }

        // this is not efficient at all
        public void DetectCollision(ref List<Entity> entityList)
        {
            // check collision between all entities
            //List<Entity> mutatedEntities = new List<Entity>(entities.Count); 
            var entities = entityList;
            for (int i = 0; i < entities.Count; i++)
            {
                if (entities[i].Entity_Type == EntityType.ENTITY_WALL) continue;

                foreach (var entity2 in entities.FindAll(s => ((s.Mask & MASK) == MASK) && s.Name != entities[i].Name))
                {
                    // loop through all other collisionable entities
                    bool collided = CalculateCollision(entities[i], entity2);

                    if (collided)
                    {
                        var currentPositionComponent = entities[i].GetPosition;
                        currentPositionComponent.RevertPosition();
                    }
                }
            }

            entityList = entities;
        }
    }
}
