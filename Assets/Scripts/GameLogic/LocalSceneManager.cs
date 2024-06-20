using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KP
{
    public class LocalSceneManager : MonoBehaviour
    {
        [SerializeField] private List<Button> playButtons;
        [SerializeField] private List<Button> pauseButtons;
        [SerializeField] private List<Button> menuButtons;
        [SerializeField] private List<Button> quitButtons;

        private void Start()
        {
            InitializeScene();
        }

        public void InitializeScene()
        {
            foreach (var playButton in playButtons)
            {
                if (playButton != null)
                {
                    playButton.onClick.AddListener(OnPlayButtonClicked);
                }
            }

            foreach (var quitButton in quitButtons)
            {
                if (quitButton != null)
                {
                    quitButton.onClick.AddListener(OnQuitButtonClicked);
                }
            }

            foreach (var pauseButton in pauseButtons)
            {
                if (pauseButton != null)
                {
                    pauseButton.onClick.AddListener(OnPauseButtonClicked);
                }
            }

            foreach (var menuButton in menuButtons)
            {
                if (menuButton != null)
                {
                    menuButton.onClick.AddListener(OnMenuButtonClicked);
                }
            }
        }

        private void OnPlayButtonClicked()
        {
            Debug.Log("Play Button Clicked");
            GameManager.instance.LoadLevel("SampleScene");
        }

        private void OnPauseButtonClicked()
        {
            Debug.Log("Pause Button Clicked");
            PauseSystem.instance.PauseGame();
        }

        private void OnMenuButtonClicked()
        {
            Debug.Log("Menu Button Clicked");
            GameManager.instance.LoadLevel("MainMenu");
        }

        private void OnQuitButtonClicked()
        {
            Debug.Log("Quit Button Clicked");
            GameManager.instance.QuitGame();
        }
    }
}