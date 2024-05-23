using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP
{
    public class PlayerMovement : MonoBehaviour
    {
        private CustomInputs input = null;
        private Rigidbody2D playerRB = null;
        private Vector2 currentVelocity = Vector2.zero; // For SmoothDamp
        private Vector2 targetVelocity = Vector2.zero;  // The target velocity for SmoothDamp
        [HideInInspector] public Vector2 moveVector = Vector2.zero;
        [Header("Basic Movement")]
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] float smoothTime = 0.1f; // SmoothDamp time

        [Header("Dash")]
        [SerializeField] float dashSpeed = 20f;
        [SerializeField] float dashDuration = 0.2f;

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
            input.Player.Dash.performed += OnDashPerformed;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
            input.Player.Dash.performed -= OnDashPerformed;
        }

        private void FixedUpdate()
        {
            Vector2 smoothedVelocity = Vector2.SmoothDamp(playerRB.velocity, targetVelocity, ref currentVelocity, smoothTime);
            playerRB.velocity = smoothedVelocity;
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            moveVector = value.ReadValue<Vector2>();
            targetVelocity = moveVector * moveSpeed; // Set the target velocity based on input
        }

        private void OnMovementCancelled(InputAction.CallbackContext value)
        {
            moveVector = Vector2.zero;
            targetVelocity = Vector2.zero; // Reset the target velocity when input is canceled
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            StartCoroutine(Dash());
        }

        private IEnumerator Dash()
        {
            float originalSpeed = moveSpeed;
            moveSpeed = dashSpeed;
            targetVelocity = moveVector * dashSpeed; // Set target velocity for dashing
            yield return new WaitForSeconds(dashDuration);
            moveSpeed = originalSpeed;
            targetVelocity = moveVector * moveSpeed; // Reset target velocity after dashing
        }

    }
}