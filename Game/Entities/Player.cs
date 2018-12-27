using GameEngine.Components;
using GameEngine.Entities;
using GameEngine.Systems;
using OpenTK;
using System;

namespace OpenGL_Game.Entities
{
    public class Player : Entity
    {
        private readonly int POINTS_FOR_MUNCH = 1, POINTS_FOR_GHOST = 200, POINTS_FOR_FRUIT = 50;
        private readonly Vector3 SpawnPoint = new Vector3(0, 1.5f, -2.5f);
        private readonly Vector3 LeftPortalPoint = new Vector3(-22.5f, 1.5f, -2.5f);
        private readonly Vector3 RightPortalPoint = new Vector3(22.5f, 1.5f, -2.5f);

        private const float poweredUpDuration = 7;
        private const float width = .8f;

        public bool IsPoweredUp { get; set; } = false;
        public short lives = 3;
        public float TimeSinceLastPowerup = 0;
        public float MoveSpeed = 0.2f;
        public int Score { get; set; } = 0;
        public override string Name { get; set; } = "Player";


        public Player()
        {            
            
            this.Entity_Type = EntityType.ENTITY_PLAYER;
        }

        public void ResetPlayerPosition()
        {
            // in the case of stuck in a wall
            OverrideComponent(new ComponentPosition(SpawnPoint));
        }

        public void RevertPlayerPosition()
        {
            var pc = (ComponentPosition)GetComponent(ComponentTypes.COMPONENT_POSITION);
            pc.RevertPosition();
        }

        public override float GetWidth()
        {
            return width;           
        }

        public override float GetDepth()
        {
            return width; 
        }

        public void UpdateTickTimer(float t)
        {
            TimeSinceLastPowerup += t;

            if (TimeSinceLastPowerup > poweredUpDuration)
                PowerDown();
        }
        
        public void PowerUp()
        {
            Score += POINTS_FOR_FRUIT;

            IsPoweredUp = true;
            MoveSpeed = 0.33f;
            TimeSinceLastPowerup = 0;

            PlayEatFruitSfx();
            StartSirenSfx();
        }

        private void PowerDown()
        {
            IsPoweredUp = false;
            MoveSpeed = 0.2f;

            StopSirenSfx();

        }

        public override void Update(float dt)
        {
            base.Update(dt);

            if (IsPoweredUp)
                UpdateTickTimer(dt);

        }

        public void Move(Vector3 cameraRotation, float x, float y, float z)
        {
            var pos = GetPosition;
            pos.LastPos = pos.Position;

            Vector3 offset = new Vector3();
            Vector3 yDelta = new Vector3((float)Math.Sin(cameraRotation.X), 0, (float)Math.Cos(cameraRotation.X));
            Vector3 XDelta = new Vector3(-yDelta.Z, 0, yDelta.X);

            offset += x * XDelta;
            offset += y * yDelta;
            offset.Y += z;

            offset.NormalizeFast();
            offset = Vector3.Multiply(offset, MoveSpeed);

            pos.Position += offset;
        }

        public void EatMunch()
        {
            Score += POINTS_FOR_MUNCH;
            PlayEatPacdotSfx();
        }

        public void EatGhost()
        {
            if(IsPoweredUp)
            {
                Score += POINTS_FOR_GHOST;
                PlayEatGhostSfx();
            }
            else
            {
                LifeLost();
            }
        }
       
        public void Teleport(string teleporter)
        {
            PlayTeleportSfx();
            if (teleporter == "leftPortal")
            {
                this.OverrideComponent(new ComponentPosition(RightPortalPoint));
            }
            else if (teleporter == "rightPortal")
            {
                this.OverrideComponent(new ComponentPosition(LeftPortalPoint));
            }
        }

        public void LifeLost()
        {
            lives--;
            if (lives < 1)
                this.IsDestroyed = true;


            PlayPacDeathSfx();

            this.OverrideComponent(new ComponentPosition(SpawnPoint));
        }

        public void StartSirenSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "siren.wav");
            if (!SystemAudio.IsPlaying(ac.Source))
            {
                SystemAudio.SetLooping(ac.Source, true); 
                SystemAudio.Start(ac.Source);
            }
        }

        public void StopSirenSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "siren.wav");
            if (SystemAudio.IsPlaying(ac.Source))
            {
                SystemAudio.Stop(ac.Source);
            }
        }

        public void PlayTeleportSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "teleport.wav");
            SystemAudio.Start(ac.Source);
        }

        public void PlayEatPacdotSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "pickup.wav");
            SystemAudio.Start(ac.Source);
        }

        public void PlayEatFruitSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "eatfruit.wav");
            SystemAudio.Start(ac.Source); 
        }

        public void PlayEatGhostSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "eatghost.wav");
            SystemAudio.Start(ac.Source);
        }

        public void PlayLifeLostSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "willhelm.wav");
            SystemAudio.Start(ac.Source);
        }

        public void PlayPacDeathSfx()
        {
            var ac = (ComponentAudio)this.GetComponent(ComponentTypes.COMPONENT_AUDIO, "death.wav");
            SystemAudio.Start(ac.Source);

        }
        public override void OnDeath()
        {
            // do spooky death things
            PlayPacDeathSfx(); 
        }

    }
}
