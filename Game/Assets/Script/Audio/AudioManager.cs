using UnityEngine;

namespace Script.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public AudioClip[] playlist;
        public AudioSource audioSource;
        private int _musicIndex;
        
        void Start()
        {
            if (playlist.Length == 0)
                return;
        
            audioSource.clip = playlist[0];
            audioSource.Play();
            
            InvokeRepeating(nameof(UpdateSong), 0, 1);
        }
        
        private void UpdateSong()
        {
            if (!audioSource.isPlaying && playlist.Length > 0)
            {
                PlayNextSong();
            }
        }

        private void PlayNextSong()
        {
            _musicIndex = (_musicIndex + 1) % playlist.Length;
            audioSource.clip = playlist[_musicIndex];
            audioSource.Play();
        }
    }
}
