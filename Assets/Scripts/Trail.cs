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

