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
    public class Pacdot : Entity
    {
        private Vector3 rotationalDelta;
        private const float Scale = .3f;
        private const float Width = 2f;

        public Pacdot(string name, double x_rotation, double y_rotation, double z_rotation)
        {
            Name = name;
            Entity_Type = EntityType.ENTITY_MUNCH;

            // give random rotation
            Random rand = new Random();
            float rotationX = (float)x_rotation;
            float rotationY = (float)y_rotation;
            float rotationZ = (float)z_rotation;
            rotationalDelta = (new Vector3(rotationX, rotationY, rotationZ));



            AddComponent(new ComponentGeometry(new Cube(new Vector3(.5f, .5f, .5f))));
            //AddComponent(new ComponentGeometry("sphere.OBJ", Scale));
            AddComponent(new ComponentTexture("blue.jpg"));
            AddComponent(new ComponentCollision());
        }
        
        public Pacdot(string name, Vector3 rotation)
        {
            Name = name;
            Entity_Type = EntityType.ENTITY_MUNCH;
            rotationalDelta = new Vector3(MathHelper.DegreesToRadians(rotation.X), MathHelper.DegreesToRadians(rotation.Y), MathHelper.DegreesToRadians(rotation.Z)); 
            
            AddComponent(new ComponentGeometry(new Cube(new Vector3(.5f, .5f, .5f))));
            AddComponent(new ComponentTexture("pacdot.jpg"));
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
