using GameEngine.Components;
using GameEngine.Entities;
using GameEngine.Managers;
using OpenTK;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;  

namespace GameEngine.Systems
{
    public class SystemAudio : ISystem
    {
        private const ComponentTypes MASK = ComponentTypes.COMPONENT_AUDIO | ComponentTypes.COMPONENT_POSITION;
        public static AudioContext Context; 

        public SystemAudio()
        {
            Context = new AudioContext(); 
        }

        public string Name
        {
            get { return "SystemAudio"; }
        }

        public void OnAction(Entity entity)
        {
            // Set/Update the audio component of the entity using its other components
            if ((entity.Mask & MASK) == MASK)
            {
                foreach(var audioComponent in entity.GetComponents(ComponentTypes.COMPONENT_AUDIO)) // enables multiple audio components per enity
                {
                    var audioC = (ComponentAudio)audioComponent;

                    
                    var posC = (ComponentPosition)entity.GetComponent(ComponentTypes.COMPONENT_POSITION);
                    var velC = (ComponentVelocity)entity.GetComponent(ComponentTypes.COMPONENT_VELOCITY);

                    SetPosition(audioC.Source, posC.Position);

                    if (velC != null)
                        SetVelocity(audioC.Source, velC.Velocity);
                }
            }
        }

        public static bool IsPlaying(int src)
        { 
            return AL.GetSourceState(src) == ALSourceState.Playing; 
        }

        public static bool IsStopped(int src)
        {
            return AL.GetSourceState(src) == ALSourceState.Stopped;
        }

        public static void SetLooping(int src, bool isLooping)
        {
            AL.Source(src, ALSourceb.Looping, isLooping);
        }

        

        public static void Start(int src)
        {
            AL.SourcePlay(src);
        }

        /// <summary>
        /// One time play method for a source.
        /// </summary>
        /// <param name="buffer"></param>
        public static void PlayGlobalInstance(int buffer, bool isLooping = false)
        {
            AL.GenSource(out uint src);
            AL.Source(src, ALSourcei.Buffer, buffer);
            AL.Source(src, ALSourceb.Looping, isLooping);
            AL.Source(src, ALSourceb.SourceRelative, false);
            AL.SourcePlay(src);
            //AL.DeleteSource((int)src);
        }

        private void Resume(int src)
        {
            AL.SourcePlay(src);
        }

        private void Pause(int src)
        {
            AL.SourcePause(src);
        }

        public static void Stop(int src)
        {
            AL.SourceStop(src);
        }

        public static void SetVolume(int src, float vol)
        {
            AL.Source(src, ALSourcef.Gain, vol);
        }

        public static void SetPitch(int src, float pitch)
        {
            AL.Source(src, ALSourcef.Pitch, pitch);
        }

        private void SetVelocity(int src, Vector3 velocity)
        {
            AL.Source(src, ALSource3f.Velocity, ref velocity);
        }

        public static void SetPosition(int src, Vector3 pos)
        {
            AL.Source(src, ALSource3f.Position, ref pos);
        }

        public static void SetPosition(int src, float x, float y, float z)
        {
            Vector3 pos = new Vector3(x, y, z);
            AL.Source(src, ALSource3f.Position, ref pos);
        }

        public static void SetListenerPosition(Vector3 position)
        {
            AL.Listener(ALListener3f.Position, ref position);
        }

        public static void DeleteSource(int src)
        {
            AL.SourceStop(src);
            AL.DeleteSource(src);
        }
        
        public static void DeleteBuffer(int buffer)
        {
            AL.DeleteBuffer(buffer); 
        }

        public void Dispose()
        {
            List<int> list = ResourceManager.audioDictionary.Values.ToList(); 
            foreach(int i in list)
            {
                DeleteBuffer(i);
            }
            Context.Dispose(); 
        }
    }
}
