using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class SFXManager : MonoBehaviour
    {
        public static SFXManager instance;

        [Header("Audio Clips")]
        public AudioClip dashSFX;
        public AudioClip enemyHitSFX;
        public AudioClip gameOverSFX;

        [Header("Audio Source")]
        private AudioSource audioSource;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public void PlayDashSFX()
        {
            PlaySound(dashSFX);
        }

        public void PlayEnemyHitSFX()
        {
            PlaySound(enemyHitSFX);
        }

        public void PlayGameOverSFX()
        {
            PlaySound(gameOverSFX);
        }

        private void PlaySound(AudioClip clip)
        {
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
