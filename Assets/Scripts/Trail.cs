using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class Trail : MonoBehaviour
    {
        [SerializeField] private float lifetime;

        private void Awake()
        {
            StartCoroutine(FadeAndDestroy());
        }

        public void Initialize(Vector3 startPosition, Vector3 endPosition)
        {
            // Set the trail position and rotation
            transform.position = startPosition;

            Vector2 direction = (endPosition - startPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Calculate the distance and set the scale
            float distance = Vector3.Distance(startPosition, endPosition);

            // Adjust the local scale along the x-axis
            transform.localScale = new Vector3(distance, transform.localScale.y, transform.localScale.z);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
                GameManager.instance.AddScore(5); // Additional score for projectiles destroyed by the shape
            }
        }

        private IEnumerator FadeAndDestroy()
        {
            float fadeTime = lifetime;
            SpriteRenderer sr = GetComponent<SpriteRenderer>();

            while (fadeTime > 0)
            {
                fadeTime -= Time.deltaTime;
                Color color = sr.color;
                color.a = Mathf.Lerp(0, 1, fadeTime / lifetime);
                sr.color = color;
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}

