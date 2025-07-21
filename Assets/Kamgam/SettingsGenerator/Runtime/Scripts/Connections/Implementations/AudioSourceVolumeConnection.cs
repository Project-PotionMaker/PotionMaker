using System.Collections.Generic;
using UnityEngine;

namespace Kamgam.SettingsGenerator
{
    public class AudioSourceVolumeConnection : Connection<float>
    {
        [Tooltip("How the input should be mapped to 0f..1f.\n" +
            "Useful if you have a range in percent (from 0 to 100) but need output ranging from 0f to 1f.")]
        public Vector2 InputRange = new Vector2(0f, 100f);

        public List<AudioSource> AudioSources = new List<AudioSource>();

        public AudioSourceVolumeConnection(Vector2 inputRange, IList<AudioSource> audioSources)
        {
            InputRange = inputRange;
            AddAudioSources(audioSources);
        }

        public void AddAudioSources(IList<AudioSource> audioSources)
        {
            if (AudioSources == null)
            {
                AudioSources = new List<AudioSource>();
            }

            if (audioSources != null)
            {
                AudioSources.AddIfNotContained(audioSources);
                DefragAudioSources();
            }
        }
        
        public void RemoveAudioSources(IList<AudioSource> audioSources)
        {
            if (AudioSources == null)
            {
                AudioSources = new List<AudioSource>();
            }

            if (audioSources != null)
            {
                AudioSources.RemoveRange(audioSources);
                DefragAudioSources();
            }
        }

        public override float Get()
        {
            DefragAudioSources();

            if (AudioSources.IsNullOrEmpty() || AudioSources[0] == null || AudioSources[0].gameObject == null)
                return MathUtils.MapWithAnchor(0.5f, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y);

            return MathUtils.MapWithAnchor(AudioSources[0].volume, 0f, 0f, 1f, InputRange.x, InputRange.x, InputRange.y, clamp: false);
        }

        public override void Set(float value)
        {
            if (AudioSources.IsNullOrEmpty())
                return;

            foreach (var source in AudioSources)
            {
                if (source == null || source.gameObject == null)
                    continue;

                source.volume = MathUtils.MapWithAnchor(value, InputRange.x, InputRange.x, InputRange.y, 0f, 0f, 1f, clamp: false);
            }
        }
        
        public void DefragAudioSources()
        {
            if (AudioSources == null)
                return;
            
            for (int i = AudioSources.Count - 1; i >= 0; i--)
            {
                if (AudioSources[i] == null || AudioSources[i].gameObject == null)
                    AudioSources.RemoveAt(i);
            }
        }
    }
}
