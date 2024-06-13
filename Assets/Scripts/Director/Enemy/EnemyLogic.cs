using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class EnemyLogic : MonoBehaviour
    {
        [SerializeField] private int damageAmount = 10; // Damage amount dealt by this enemy
        private Health enemyHealth;

        private void Awake()
        {
            enemyHealth = GetComponent<Health>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                playerHealth.Damage(damageAmount);
                Destroy(gameObject);
            }
            else if (collision.gameObject.CompareTag("Trail")) {
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
