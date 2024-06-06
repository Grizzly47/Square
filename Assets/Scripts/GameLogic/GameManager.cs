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
        

        [Header("Gameplay")]
        private int score;

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
            score = 0;        }

        public void AddScore(int points)
        {
            score += points;
            Debug.Log("Score: " + score);
            UIManager.instance.UpdateScore(score);
        }

        public int GetScore()
        {
            return score;
        }

        public void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }

        public void ReloadCurrentLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}