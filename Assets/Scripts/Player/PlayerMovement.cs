using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP
{
    public class PlayerMovement : MonoBehaviour
    {
        private CustomInputs input = null;
        private Rigidbody2D playerRB = null;
        private BoxCollider2D playerCollider;
        private Vector2 currentVelocity = Vector2.zero;
        private Vector2 targetVelocity = Vector2.zero;
        [HideInInspector] public Vector2 moveVector = Vector2.zero;

        private Transform visualTransform;
        private Vector3 originalScale;
        private Vector3 targetScale;
        private bool isDashing = false;

        [Header("Basic Movement")]
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] float smoothTime = 0.1f;

        [Header("Dash")]
        [SerializeField] float dashSpeed = 20f;
        [SerializeField] float dashDuration = 0.2f;

        [Header("Squash and Stretch")]
        [SerializeField] float moveSquashAmount = 0.1f;
        [SerializeField] float dashSquashAmount = 0.2f;
        [SerializeField] float squashSmoothTime = 0.1f;

        private void Awake()
        {
            input = new CustomInputs();
            playerRB = GetComponent<Rigidbody2D>();
            playerRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            playerCollider = GetComponent<BoxCollider2D>();
            visualTransform = transform.Find("Sprite");
            originalScale = visualTransform.localScale;
            targetScale = originalScale;
            if (TrailManager.instance != null)
            {
                EdgeCollider2D edgeCollider = TrailManager.instance.GetEdgeCollider();
                if (edgeCollider != null)
                {
                    Physics2D.IgnoreCollision(playerCollider, edgeCollider);
                }
            }
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

            RotateCharacter(smoothedVelocity);
            ApplySquashAndStretch(smoothedVelocity);
        }

        private void RotateCharacter(Vector2 velocity)
        {
            if (velocity != Vector2.zero)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                playerRB.rotation = angle;
            }
        }

        private void ApplySquashAndStretch(Vector2 velocity)
        {
            float speed = velocity.magnitude;
            if (speed > 0)
            {
                float squashFactor = 1 + moveSquashAmount * speed / moveSpeed;
                targetScale = new Vector3(originalScale.x * squashFactor, originalScale.y / squashFactor, originalScale.z);
            }
            else
            {
                targetScale = originalScale;
            }
            visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, targetScale, squashSmoothTime);
        }

        private void OnMovementPerformed(InputAction.CallbackContext value)
        {
            //if (!isDashing)
            //{
                moveVector = value.ReadValue<Vector2>();
                targetVelocity = moveVector * moveSpeed;
            //}
        }

        private void OnMovementCancelled(InputAction.CallbackContext value)
        {
            //if (!isDashing)
            //{
                moveVector = Vector2.zero;
                targetVelocity = Vector2.zero;
            //}
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (!isDashing)
            {
                StartCoroutine(Dash());
            }
        }

        private IEnumerator Dash()
        {
            isDashing = true;
            float originalSpeed = moveSpeed;
            Vector2 initialDashDirection = moveVector.normalized;
            float currentDashSpeed = dashSpeed; // Start from dash speed
            float maxDashDistance = 25f; // Max distance for the dash
            Vector3 startPosition = transform.position;
            float dashTimeElapsed = 0f;
            bool shapeCompleted = false;
            float pointRadius = TrailManager.instance.GetPointRadius(); // Get pointRadius from TrailManager

            // Set initial squash and stretch
            targetScale = new Vector3(originalScale.x * (1 + dashSquashAmount), originalScale.y / (1 + dashSquashAmount), originalScale.z);
            visualTransform.localScale = targetScale;

            // Smoothly accelerate
            while (dashTimeElapsed < dashDuration / 2 && Vector3.Distance(startPosition, transform.position) < maxDashDistance)
            {
                dashTimeElapsed += Time.deltaTime;
                currentDashSpeed = Mathf.Lerp(0, dashSpeed, dashTimeElapsed / (dashDuration / 2)); // Smoothly accelerate

                playerRB.velocity = initialDashDirection * currentDashSpeed;

                Vector3 currentPosition = transform.position;

                // Raycast to detect obstacles
                RaycastHit2D hit = Physics2D.Raycast(currentPosition, initialDashDirection, currentDashSpeed * Time.deltaTime);
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    // Stop the dash if a wall is detected
                    break;
                }

                // Check if the player has passed the start point
                if (!shapeCompleted && Vector3.Distance(currentPosition, startPosition) <= pointRadius)
                {
                    currentPosition = startPosition; // Snap to start point
                    shapeCompleted = true;
                }

                TrailManager.instance.AddTrailPoint(currentPosition);

                yield return null;
            }

            // Smoothly decelerate
            while (dashTimeElapsed < dashDuration && Vector3.Distance(startPosition, transform.position) < maxDashDistance)
            {
                dashTimeElapsed += Time.deltaTime;
                currentDashSpeed = Mathf.Lerp(dashSpeed, originalSpeed, (dashTimeElapsed - dashDuration / 2) / (dashDuration / 2)); // Smoothly decelerate to original speed

                playerRB.velocity = initialDashDirection * currentDashSpeed;

                Vector3 currentPosition = transform.position;

                // Raycast to detect obstacles
                RaycastHit2D hit = Physics2D.Raycast(currentPosition, initialDashDirection, currentDashSpeed * Time.deltaTime);
                if (hit.collider != null && hit.collider.CompareTag("Wall"))
                {
                    // Stop the dash if a wall is detected
                    break;
                }

                // Check if the player has passed the start point
                if (!shapeCompleted && Vector3.Distance(currentPosition, startPosition) <= pointRadius)
                {
                    currentPosition = startPosition; // Snap to start point
                    shapeCompleted = true;
                }

                TrailManager.instance.AddTrailPoint(currentPosition);

                yield return null;
            }

            // Ensure the player continues moving at original speed
            moveSpeed = originalSpeed;
            targetVelocity = moveVector * moveSpeed;

            // Smoothly return to original scale
            float squashTimeElapsed = 0f;
            while (squashTimeElapsed < squashSmoothTime)
            {
                squashTimeElapsed += Time.deltaTime;
                visualTransform.localScale = Vector3.Lerp(visualTransform.localScale, originalScale, squashTimeElapsed / squashSmoothTime);
                yield return null;
            }

            isDashing = false;

            TrailManager.instance.CheckForShapes();
        }
    }
}


