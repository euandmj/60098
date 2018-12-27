using GameEngine.Systems;
using GameEngine.Managers;
using OpenTK.Audio.OpenAL;

namespace GameEngine.Components
{
    public class ComponentAudio : IComponent
    {
        public string Name { get; internal set; } = "ComponentAudio";

        public ComponentAudio(string path,  bool loop = false, bool paused = true)
        {
            Name = path;
            isLooping = loop;
            isPaused = paused;

            //GenSource(ref SystemAudio.Context);
            //using (var aContext = SystemAudio.Context)
            //    Source = AL.GenSource();

            Buffer = ResourceManager.GetAudio(path);
            GenSource(ref SystemAudio.Context);

            SystemAudio.SetLooping(Source, isLooping);

            if (!isPaused)
                SystemAudio.Start(Source);
        }

        public int Source = -1;
        public int Buffer = -1;

        public bool isPaused { get; set; }
        public bool isLooping { get; set; }
        
        public ComponentTypes ComponentType
        {
            get { return ComponentTypes.COMPONENT_AUDIO; }
        }           

        public void OnDestroy()
        {
            SystemAudio.DeleteSource(this.Source); 
        }

        private void GenSource(ref OpenTK.Audio.AudioContext context)
        {           
            Source = AL.GenSource();
            AL.Source(Source, ALSourcei.Buffer, Buffer);
        }
    }
}
