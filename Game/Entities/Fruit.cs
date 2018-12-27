using System;
using System.Collections.Generic;
using GameEngine.Systems;
using System.Linq;
using System.Text;
using GameEngine.Components;
using OpenTK;
using GameEngine.Entities;
using GameEngine.Geometry;

namespace OpenGL_Game.Entities
{
    public class Fruit : Entity
    {
        private const float Width = 2f;
        private Vector3 rotationalDelta;

        public Fruit(string name, int rotation)
        {

            rotationalDelta = new Vector3(0, MathHelper.DegreesToRadians(rotation), 0);
            Name = name;
            Entity_Type = EntityType.ENTITY_FRUIT;


            // Default components per Fruit
            AddComponent(new ComponentGeometry(new Pyramid(.5f, 1)));
            AddComponent(new ComponentTexture("yellowRupee.png"));
            AddComponent(new ComponentCollision());
        }



        public override void Update(float dt)
        {
            base.Update(dt);


            var geoemetry = (ComponentGeometry)GetComponent(ComponentTypes.COMPONENT_GEOMETRY);
            geoemetry._Object.Rotation += rotationalDelta * dt;

        }

        public override float GetWidth()
        {
            return Width;
        }

        public override float GetDepth()
        {
            return Width;
        }

        public override float GetRadius()
        {
            return Width;
        }
    }
}
