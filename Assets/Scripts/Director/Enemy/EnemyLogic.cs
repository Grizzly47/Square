using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class EnemyLogic : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 10; // Damage amount dealt by this enemy
        private Health enemyHealth;
        private Rigidbody2D enemyRB;

        private void Awake()
        {
            enemyHealth = GetComponent<Health>();
            enemyRB = GetComponent<Rigidbody2D>();

            // Ignore collisions with other enemies
            Collider2D[] colliders = GetComponents<Collider2D>();
            foreach (Collider2D col in colliders)
            {
                Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
            }

            enemyRB.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    SFXManager.instance.PlayEnemyHitSFX();
                    playerHealth.Damage(damageAmount);
                    Debug.Log("Damage dealt");
                }
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Trail"))
            {
                GameManager.instance.AddScore(10);
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}