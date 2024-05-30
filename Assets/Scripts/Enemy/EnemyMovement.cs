using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 5f; // Speed of enemy movement
        private Rigidbody2D rb;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            // Set initial velocity to move horizontally ( add vertical movement later)
            rb.velocity = Vector2.left * speed;
        }
    }
}