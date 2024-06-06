using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace KP
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        [SerializeField] private TMP_Text scoreText;

        private void Awake()
        {
            // Singleton pattern to ensure only one instance of UIManager exists
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

        public void UpdateScore(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
            }
        }
    }
}