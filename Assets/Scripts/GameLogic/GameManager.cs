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
            score = 0;        }

        public void AddScore(int points)
        {
            score += points*multiplier;
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