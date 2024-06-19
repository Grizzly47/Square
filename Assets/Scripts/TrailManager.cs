using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class TrailManager : MonoBehaviour
    {
        public static TrailManager instance;

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

            lineRenderer.positionCount = 0;
            edgeCollider.points = new Vector2[0];

            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
            IgnorePlayerCollision();
        }

        private void IgnorePlayerCollision()
        {
            int playerLayer = LayerMask.NameToLayer("Player");
            int trailLayer = LayerMask.NameToLayer("Trail");

            if (playerLayer == -1 || trailLayer == -1)
            {
                Debug.LogError("Layer not found! Please check layer names.");
                return;
            }

            Physics2D.IgnoreLayerCollision(playerLayer, trailLayer, true);
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
            // New Dash Starts too far away
            if (trailPositions.Count > 0 && Vector3.Distance(position, trailPositions[trailPositions.Count - 1]) > maxDistanceBetweenDashes)
            {
                trailPositions.Clear();
                trailTimes.Clear();
                lineRenderer.positionCount = 0;
                edgeCollider.points = new Vector2[0];
            }

            // New dash point is close enough
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
            // Define the expected sides and margin for shape detection
            int[] expectedSidesArray = new int[] { 3, 4, 5 }; // Example: triangle, square, pentagon
            float[] angleThresholds = new float[] { 120f, 90f, 72f }; // Corresponding angle thresholds for shapes
            float angleMargin = 15f; // Allowable margin of error in degrees

            for (int j = 0; j < expectedSidesArray.Length; j++)
            {
                int expectedSides = expectedSidesArray[j];
                float idealAngle = angleThresholds[j];
                float lowerThreshold = idealAngle - angleMargin;
                float upperThreshold = idealAngle + angleMargin;

                List<Vector3> significantPoints = GetSignificantPoints(trailPositions, lowerThreshold);

                if (significantPoints.Count >= expectedSides)
                {
                    float distanceToFirstPoint = Vector3.Distance(significantPoints[significantPoints.Count - 1], significantPoints[0]);
                    if (distanceToFirstPoint <= pointRadius)
                    {
                        // Shape formed, handle shape detection logic here
                        Debug.Log("Shape formed with " + expectedSides + " sides!");
                        GameManager.instance.SetMultiplier(expectedSides);
                        return; // Exit once a valid shape is found
                    }
                }
            }

            // If no shape is detected, reset the multiplier
            GameManager.instance.SetMultiplier(1);
        }



        private float GetIdealAngleForSides(int sides)
        {
            if (sides < 3) return 180f; // Degenerate case, not a shape

            // The internal angle of a regular polygon with `sides` sides
            return 180f - (360f / sides);
        }


        private List<Vector3> GetSignificantPoints(List<Vector3> points, float angleThreshold)
        {
            List<Vector3> significantPoints = new List<Vector3>();
            if (points.Count < 3) return points; // Not enough points to form a shape

            significantPoints.Add(points[0]); // Add the first point

            for (int i = 1; i < points.Count - 1; i++)
            {
                Vector3 prevPoint = points[i - 1];
                Vector3 currPoint = points[i];
                Vector3 nextPoint = points[i + 1];

                Vector3 dir1 = (currPoint - prevPoint).normalized;
                Vector3 dir2 = (nextPoint - currPoint).normalized;

                float angle = Vector3.Angle(dir1, dir2);

                if (angle >= angleThreshold)
                {
                    significantPoints.Add(currPoint); // Add significant point
                }
            }

            significantPoints.Add(points[points.Count - 1]); // Add the last point

            return significantPoints;
        }


    }
}
