using UnityEngine;
using UnityEngine.SceneManagement;

namespace KP
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager instance;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip menuMusic;
        [SerializeField] private AudioClip gameplayMusic;

        [Header("Audio Source")]
        [SerializeField] private AudioSource audioSource;

        [Header("Volume Settings")]
        [SerializeField] private float normalVolume = 1.0f;
        [SerializeField] private float pausedVolume = 0.3f;

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
            SceneManager.sceneLoaded += OnSceneLoaded;
            PlayMusicBasedOnScene(SceneManager.GetActiveScene().name);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            PlayMusicBasedOnScene(scene.name);
        }

        private void PlayMusicBasedOnScene(string sceneName)
        {
            if (sceneName == "MainMenu")
            {
                PlayMusic(menuMusic);
            }
            else if (sceneName == "SampleScene")
            {
                PlayMusic(gameplayMusic);
            }
        }

        public void SetPaused(bool isPaused)
        {
            audioSource.volume = isPaused ? pausedVolume : normalVolume;
        }

        private void PlayMusic(AudioClip clip)
        {
            if (audioSource.clip != clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }
    }
}


