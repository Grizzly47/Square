using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP
{
    public class PauseSystem : MonoBehaviour
    {
        public static PauseSystem instance;
        private CustomInputs input = null;
        [SerializeField] GameObject pauseMenu = null;
        public bool isPaused;


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                input = new CustomInputs();
                //DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            input.Enable();
            input.UI.Pause.performed += OnPause;
        }

        private void OnDisable()
        {
            input.UI.Pause.performed -= OnPause;
            input.Disable();
        }

        private void OnPause(InputAction.CallbackContext context)
        {
            PauseGame();
        }

        public void OnPauseToggled()
        {
            if (PauseSystem.instance.isPaused)
            {
                input.Disable();
            }
            else
            {
                input.Enable();
            }
        }

        public void PauseGame()
        {
            isPaused = !isPaused;
            MusicManager.instance.SetPaused(isPaused);
            Time.timeScale = isPaused ? 0 : 1;
            pauseMenu.SetActive(isPaused);
        }
    }
}