using System.Collections.Generic;
using UnityEngine;

namespace com.mystery_mist.core
{

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager s_Instance { get; private set; }

        [SerializeField] private AudioSource backgroundMusicSource; // For background music
        [SerializeField] private AudioSource soundEffectsSource;    // For sound effects
        [SerializeField] private List<AudioClip> audioClips;         // Preloaded audio clips

        private Dictionary<string, AudioClip> audioClipDictionary;

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                InitializeAudioClips();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeAudioClips()
        {
            audioClipDictionary = new Dictionary<string, AudioClip>();
            foreach (var clip in audioClips)
            {
                if (clip != null && !audioClipDictionary.ContainsKey(clip.name))
                {
                    audioClipDictionary.Add(clip.name, clip);
                }
            }
        }

        /// <summary>
        /// Plays background music with optional loop.
        /// </summary>
        /// <param name="clipName">Name of the audio clip.</param>
        /// <param name="loop">Should the music loop?</param>
        public void PlayBackgroundMusic(string clipName, bool loop = true)
        {
            if (audioClipDictionary.TryGetValue(clipName, out var clip))
            {
                backgroundMusicSource.clip = clip;
                backgroundMusicSource.loop = loop;
                backgroundMusicSource.Play();
            }
            else
            {
                Debug.LogWarning($"AudioManager: Background music '{clipName}' not found!");
            }
        }

        /// <summary>
        /// Stops the background music.
        /// </summary>
        public void StopBackgroundMusic()
        {
            if (backgroundMusicSource.isPlaying)
            {
                backgroundMusicSource.Stop();
            }
        }

        /// <summary>
        /// Plays a sound effect.
        /// </summary>
        /// <param name="clipName">Name of the audio clip.</param>
        public void PlaySoundEffect(string clipName)
        {
            if (audioClipDictionary.TryGetValue(clipName, out var clip))
            {
                soundEffectsSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning($"AudioManager: Sound effect '{clipName}' not found!");
            }
        }

        /// <summary>
        /// Adjusts the background music volume.
        /// </summary>
        /// <param name="volume">Volume level (0.0 to 1.0).</param>
        public void SetBackgroundMusicVolume(float volume)
        {
            backgroundMusicSource.volume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Adjusts the sound effects volume.
        /// </summary>
        /// <param name="volume">Volume level (0.0 to 1.0).</param>
        public void SetSoundEffectsVolume(float volume)
        {
            soundEffectsSource.volume = Mathf.Clamp01(volume);
        }

        /// <summary>
        /// Stops all audio playback.
        /// </summary>
        public void StopAllAudio()
        {
            backgroundMusicSource.Stop();
            soundEffectsSource.Stop();
        }
    }
}
