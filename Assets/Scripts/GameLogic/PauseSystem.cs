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
        bool isPaused;


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

        public void PauseGame()
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0 : 1;
            pauseMenu.SetActive(isPaused);
        }
    }
}