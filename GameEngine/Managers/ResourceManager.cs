using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using System.IO;
using OpenTK.Audio.OpenAL;
using GameEngine.Utility;
using System.Drawing.Imaging;
using System.Drawing;
using GameEngine.Systems;
using GameEngine.Geometry;

namespace GameEngine.Managers
{
    public static class ResourceManager
    {
        static Dictionary<string, int> textureDictionary = new Dictionary<string, int>();
        static Dictionary<string, Material> materialDictionary = new Dictionary<string, Material>();
        static Dictionary<string, Model> modelDictionary = new Dictionary<string, Model>();
        public static Dictionary<string, int> audioDictionary = new Dictionary<string, int>();

        public static int gl_buffer;
        public static int gl_skybuffer;

        public struct AudioFile
        {
            public ALFormat sound_format;
            public int bufferID;
            public byte[] waveData;
            public int channels, bits_per_sample, sample_rate;
        }

        //public static Geometry LoadGeometry(string filename)
        //{
        //    Geometry geometry;
        //    geometryDictionary.TryGetValue(filename, out geometry);
        //    if (geometry == null)
        //    {
        //       // geometry = new Geometry();
        //        geometry.LoadObject(filename);
        //        geometryDictionary.Add(filename, geometry);
        //    }

        //    return geometry;
        //}

        public static int GetAudio(string filename)
        {
            // returns the buffer ID 
            if (!audioDictionary.ContainsKey(filename))
                LoadAudio(filename);

            // the file should be loaded or error handles within load audio. 
            return audioDictionary[filename];
        }

        public static void LoadAudio(string path)
        {
            AudioFile af = new AudioFile();

            af.waveData = LoadWave(
            File.Open("Assets/Audio/" + path, FileMode.Open),
            out af.channels,
            out af.bits_per_sample,
            out af.sample_rate);

            LoadFormat(ref af);

            // bind the buffer data to the 
            af.bufferID = AL.GenBuffer();
            AL.BufferData(af.bufferID, af.sound_format, af.waveData, af.waveData.Length, af.sample_rate);

            if (AL.GetError() != ALError.NoError)
            {
                ALError err = AL.GetError();
                Console.WriteLine(AL.GetError().ToString());
            }


            audioDictionary[path] = af.bufferID;
        }

        private static byte[] LoadWave(Stream stream, out int channels, out int bits, out int rate)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (BinaryReader reader = new BinaryReader(stream))
            {
                // RIFF header
                string signature = new string(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                int riff_chunck_size = reader.ReadInt32();

                string format = new string(reader.ReadChars(4));
                if (format != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE header
                string format_signature = new string(reader.ReadChars(4));
                if (format_signature != "fmt ")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int format_chunk_size = reader.ReadInt32();
                int audio_format = reader.ReadInt16();
                int num_channels = reader.ReadInt16();
                int sample_rate = reader.ReadInt32();
                int byte_rate = reader.ReadInt32();
                int block_align = reader.ReadInt16();
                int bits_per_sample = reader.ReadInt16();

                string data_signature = new string(reader.ReadChars(4));
                if (data_signature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int data_chunk_size = reader.ReadInt32();

                channels = num_channels;
                bits = bits_per_sample;
                rate = sample_rate;

                return reader.ReadBytes((int)reader.BaseStream.Length);
            }
        }

        private static void LoadFormat(ref AudioFile file)
        {
            // spooky ternary operator. not really sure how this works
            // should take parameters to adjust it?

            file.sound_format =
               file.channels == 1 && file.bits_per_sample == 8 ? ALFormat.Mono8 :
               file.channels == 1 && file.bits_per_sample == 16 ? ALFormat.Mono16 :
               file.channels == 2 && file.bits_per_sample == 8 ? ALFormat.Stereo8 :
               file.channels == 2 && file.bits_per_sample == 16 ? ALFormat.Stereo16 :
               0;
        }

        public static Model GetModel(string name)
        {
            if (!modelDictionary.TryGetValue(name, out Model model))
            {
                if (!File.Exists(name))
                    throw new FileNotFoundException();

                AddModel(ModelUtility.LoadModel(name));
                return GetModel(name);
            }

            return model;
        }

        private static void AddModel(Model model)
        {
            modelDictionary[model.Name] = model;
        }

        public static Material GetMaterial(string filepath)
        {
            if (!materialDictionary.TryGetValue(filepath, out Material material))
            {
                if (!File.Exists(filepath))
                    throw new FileNotFoundException();

                AddMaterial(MaterialUtility.LoadMaterial(filepath));
                return GetMaterial(filepath);
            }

            return material;
        }

        private static void AddMaterial(List<Material> materials)
        {
            foreach (var mat in materials)
            {
                materialDictionary[mat.Name] = mat;
            }
        }

        public static int LoadTexture2D(string filename)
        {
            filename = "Assets/Textures/" + filename;
            if (textureDictionary.ContainsKey(filename))
                return textureDictionary[filename];

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            GL.GenTextures(1, out int texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
           
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            textureDictionary[filename] = texture;
            return texture;
        }

        public static int LoadTextureAlphaOnly(string filename)
        {
            filename = "Assets/Textures/" + filename;
            if (textureDictionary.ContainsKey(filename))
                return textureDictionary[filename];

            if (!File.Exists(filename))
                throw new FileNotFoundException();

            GL.GenTextures(1, out int texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            Bitmap bmp = new Bitmap(filename);
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Alpha, PixelType.UnsignedByte, bmp_data.Scan0);

            bmp.UnlockBits(bmp_data);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            textureDictionary[filename] = texture;
            return texture;
        }    

        public static int LoadTextureCubeMap(string[] paths)
        {
            if (textureDictionary.ContainsKey("skybox"))
                return textureDictionary["skybox"];

            foreach (string s in paths)
                if (!File.Exists("Assets/Textures/" + s))
                    throw new FileNotFoundException();

            GL.GenTextures(1, out int texId);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.TextureCubeMap, texId);

            for (int i = 0; i < paths.Count(); i++)
            {
                Bitmap bmp = new Bitmap("Assets/Textures/" + paths[i]);
                BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i,
                    0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
            }
            // set texture parameters for the cubemap.
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);
            GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);


            textureDictionary.Add("skybox", texId);
            return texId;
        }

        /* Delete GL buffers */
        public static void CleanUpAudio()
        {
            foreach (var entry in audioDictionary)
            {
                SystemAudio.DeleteBuffer(entry.Value);
            }
            audioDictionary.Clear(); 
        }

        public static void CleanUpTextures()
        {
            foreach (var entry in textureDictionary)
            {
                GL.DeleteTexture(entry.Value);
            }
            textureDictionary.Clear(); 
        }
    }
}
