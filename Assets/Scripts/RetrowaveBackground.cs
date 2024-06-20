using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KP
{
    public class RetrowaveBackground : MonoBehaviour
    {
        public List<RawImage> rawImages; // List of RawImage components
        public float speed = 0.1f;
        public float minScale = 0.1f;
        public float maxScale = 1.0f;

        private float[] currentScales;

        void Start()
        {
            // Initialize scales array based on the current scale of each RawImage
            currentScales = new float[rawImages.Count];
            for (int i = 0; i < rawImages.Count; i++)
            {
                currentScales[i] = rawImages[i].rectTransform.localScale.x;
            }
        }

        void Update()
        {
            // Animate each RawImage
            for (int i = 0; i < rawImages.Count; i++)
            {
                currentScales[i] += speed * Time.deltaTime;
                if (currentScales[i] > maxScale)
                {
                    currentScales[i] = minScale; // Reset to max scale when it reaches min scale
                }

                rawImages[i].rectTransform.localScale = new Vector3(currentScales[i], currentScales[i], 1);
            }
        }
    }
}