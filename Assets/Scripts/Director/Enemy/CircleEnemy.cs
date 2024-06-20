using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class CircleEnemy : MonoBehaviour
    {
        [SerializeField] private float speed = 1.0f; // Speed of the enemy
        private Transform player; // Reference to the player's transform
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Update()
        {
            if (player != null)
            {
                // Move towards the player
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
        }

        public void SetSpeed(float newSpeed)
        {
            speed = newSpeed;

            // Update the velocity with the new speed
            if (player != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                rb.velocity = direction * speed;
            }
        }
    }
}
