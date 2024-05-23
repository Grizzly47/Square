using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP { 
    public class PlayerMovement : MonoBehaviour
    {
        private CustomInputs input = null;
        private Vector2 moveVector = Vector2.zero;
        private Rigidbody2D playerRB = null;
        [SerializeField] float moveSpeed = 10f;


        private void Awake()
        {
            input = new CustomInputs();
            playerRB = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            input.Enable();
            input.Player.Movement.performed += OnMovementPerformed;
            input.Player.Movement.canceled += OnMovementCancelled;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
        }

        private void FixedUpdate()
        {
            playerRB.velocity = moveVector * moveSpeed;
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            moveVector = value.ReadValue<Vector2>();
        }

        private void OnMovementCancelled(InputAction.CallbackContext value)
        {
            moveVector = Vector2.zero;

        }
    }
}