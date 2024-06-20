using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KP
{
    public class GameManager : MonoBehaviour
    {
        [Header("Instance")]
        public static GameManager instance;

        [Header("UI")]
        // Your UI related variables

        [Header("Gameplay")]
        [SerializeField] private GameObject playerPrefab;
        private int score;
        private int multiplier = 1;

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
            score = 0;
        }

        public void AddScore(int points)
        {
            score += points * multiplier;
            Debug.Log("Score: " + score);
            UIManager.instance.UpdateScore(score);
        }

        public int GetScore()
        {
            return score;
        }

        public void SetMultiplier(int _newMultiplier)
        {
            multiplier = _newMultiplier;
            UIManager.instance.UpdateMultilpier(multiplier);
        }

        public int GetMultiplier()
        {
            return multiplier;
        }

        public void LoadLevel(string levelName)
        {
            StartCoroutine(LoadLevelCoroutine(levelName));
        }

        private IEnumerator LoadLevelCoroutine(string levelName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

            // Wait until the scene is loaded
            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            Time.timeScale = 1;

            if (levelName == "SampleScene")
            {
                score = 0;
                multiplier = 1;
                GameObject player = GameObject.FindWithTag("Player");
                if (player == null)
                {
                    Debug.Log("Instantiating Player");
                    player = Instantiate(playerPrefab);
                }
                OnPlayerInstantiated(player);
            }

            // Initialize the local scene manager after the scene is loaded
            LocalSceneManager localSceneManager = FindObjectOfType<LocalSceneManager>();
            if (localSceneManager != null)
            {
                localSceneManager.InitializeScene();
            }
        }

        public void ReloadCurrentLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void OnPlayerInstantiated(GameObject player)
        {
            Health playerHealth = player.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.Died.AddListener(OnPlayerDied);
            }
        }

        private void OnPlayerDied()
        {
            Debug.Log("Player died!");
            SFXManager.instance.PlayGameOverSFX();
            GameManager.instance.LoadLevel("GameOver");
        }
    }
}