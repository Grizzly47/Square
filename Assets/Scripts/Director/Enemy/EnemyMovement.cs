using UnityEngine;

namespace KP
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f; // Speed of enemy movement
        [SerializeField] private Vector2 direction = new Vector2(1, 1); // Direction of movement (45 degrees)
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();

            // Normalize the direction to ensure consistent speed
            direction = direction.normalized;

            // Set initial velocity based on the direction and speed
            rb.velocity = direction * speed;
        }

        public void SetDirection(Vector2 newDirection)
        {
            direction = newDirection.normalized;

            // Update the velocity with the new direction
            if (rb != null)
            {
                rb.velocity = direction * speed;
            }
        }

        private void FixedUpdate()
        {
            RotateCharacter(rb.velocity);
        }

        private void RotateCharacter(Vector2 velocity)
        {
            if (velocity != Vector2.zero)
            {
                // Calculate the angle from the velocity vector
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;

                // Apply the rotation to the transform
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
            }
        }
    }
}

