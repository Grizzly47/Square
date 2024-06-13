using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class TrailManager : MonoBehaviour
    {
        public static TrailManager instance { get; private set; }

        [Header("Trail Settings")]
        [SerializeField] private GameObject trailColliderObject;
        [SerializeField] private float pointRadius = 0.5f;
        [SerializeField] private float maxDistanceBetweenDashes = 1.0f;
        [SerializeField] private float trailLifetime = 2f;

        private LineRenderer lineRenderer;
        private EdgeCollider2D edgeCollider;
        private List<Vector3> trailPositions = new List<Vector3>();
        private List<float> trailTimes = new List<float>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            lineRenderer = GetComponent<LineRenderer>();
            edgeCollider = trailColliderObject.GetComponent<EdgeCollider2D>();

            if (lineRenderer == null || edgeCollider == null)
            {
                Debug.LogError("LineRenderer or EdgeCollider2D not found.");
                return;
            }

            lineRenderer.positionCount = 0;
            edgeCollider.points = new Vector2[0];

            // Set the width of the LineRenderer
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }

        public EdgeCollider2D GetEdgeCollider()
        {
            return edgeCollider;
        }

        public float GetPointRadius()
        {
            return pointRadius;
        }

        public void AddTrailPoint(Vector3 position)
        {
            if (trailPositions.Count > 0 && Vector3.Distance(position, trailPositions[trailPositions.Count - 1]) > maxDistanceBetweenDashes)
            {
                trailPositions.Clear();
                trailTimes.Clear();
                lineRenderer.positionCount = 0;
                edgeCollider.points = new Vector2[0];
            }

            if (trailPositions.Count == 0 || Vector3.Distance(position, trailPositions[trailPositions.Count - 1]) > pointRadius)
            {
                trailPositions.Add(position);
                trailTimes.Add(Time.time);

                lineRenderer.positionCount = trailPositions.Count;
                lineRenderer.SetPositions(trailPositions.ToArray());
                UpdateEdgeCollider();
            }
        }

        private void UpdateTrail()
        {
            if (trailPositions.Count == 0) return;

            float currentTime = Time.time;

            for (int i = trailPositions.Count - 1; i >= 0; i--)
            {
                float timeSinceAdded = currentTime - trailTimes[i];
                if (timeSinceAdded > trailLifetime)
                {
                    trailPositions.RemoveAt(i);
                    trailTimes.RemoveAt(i);
                }
            }

            lineRenderer.positionCount = trailPositions.Count;
            lineRenderer.SetPositions(trailPositions.ToArray());
            UpdateEdgeCollider();
        }

        private void UpdateEdgeCollider()
        {
            Vector2[] colliderPoints = new Vector2[trailPositions.Count];
            for (int i = 0; i < trailPositions.Count; i++)
            {
                Vector3 pos = trailPositions[i];
                colliderPoints[i] = new Vector2(pos.x, pos.y);
            }
            edgeCollider.points = colliderPoints;
        }

        public void CheckForShapes()
        {
            if (trailPositions.Count >= 3)
            {
                float distanceToFirstPoint = Vector3.Distance(trailPositions[trailPositions.Count - 1], trailPositions[0]);
                if (distanceToFirstPoint <= pointRadius)
                {
                    // Shape formed, handle shape detection logic here
                    Debug.Log("Shape formed!");
                }
            }
        }
    }
}
