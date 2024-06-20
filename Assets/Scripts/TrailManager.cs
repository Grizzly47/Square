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
        [SerializeField] private float shapeLifetime = 5f; // Lifetime for points forming a shape

        private LineRenderer lineRenderer;
        private EdgeCollider2D edgeCollider;
        private List<TrailPoint> trailPoints = new List<TrailPoint>();

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
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

        private void Update()
        {
            UpdateTrail();
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
            if (trailPoints.Count > 0 && Vector3.Distance(position, trailPoints[trailPoints.Count - 1].Position) > maxDistanceBetweenDashes)
            {
                trailPoints.Clear();
                lineRenderer.positionCount = 0;
                edgeCollider.points = new Vector2[0];
            }

            // New dash point is close enough
            if (trailPoints.Count == 0 || Vector3.Distance(position, trailPoints[trailPoints.Count - 1].Position) > pointRadius)
            {
                trailPoints.Add(new TrailPoint(position, trailLifetime));

                lineRenderer.positionCount = trailPoints.Count;
                lineRenderer.SetPositions(trailPoints.ConvertAll(p => p.Position).ToArray());
                UpdateEdgeCollider();
            }
        }

        private void UpdateTrail()
        {
            if (trailPoints.Count == 0)
            {
                // Clear the collider points if there are no trail points
                edgeCollider.points = new Vector2[0];
                lineRenderer.positionCount = 0;
                return;
            }

            for (int i = trailPoints.Count - 1; i >= 0; i--)
            {
                trailPoints[i].Lifetime -= Time.deltaTime;
                if (trailPoints[i].Lifetime <= 0)
                {
                    trailPoints.RemoveAt(i);
                }
            }

            if (trailPoints.Count == 0)
            {
                // Clear the collider points if all trail points have expired
                edgeCollider.points = new Vector2[0];
                lineRenderer.positionCount = 0;
                return;
            }

            lineRenderer.positionCount = trailPoints.Count;
            lineRenderer.SetPositions(trailPoints.ConvertAll(p => p.Position).ToArray());

            // Update line renderer colors to fade out
            if (trailPoints.Count > 0)
            {
                int gradientKeyCount = Mathf.Min(8, trailPoints.Count);
                Gradient gradient = new Gradient();
                GradientColorKey[] colorKeys = new GradientColorKey[gradientKeyCount];
                GradientAlphaKey[] alphaKeys = new GradientAlphaKey[gradientKeyCount];

                for (int i = 0; i < gradientKeyCount; i++)
                {
                    float t = (float)i / (gradientKeyCount - 1);
                    int pointIndex = Mathf.Clamp(Mathf.FloorToInt(t * (trailPoints.Count - 1)), 0, trailPoints.Count - 1);
                    float alpha = trailPoints[pointIndex].GetAlpha();

                    colorKeys[i] = new GradientColorKey(Color.white, t);
                    alphaKeys[i] = new GradientAlphaKey(alpha, t);
                }

                gradient.SetKeys(colorKeys, alphaKeys);
                lineRenderer.colorGradient = gradient;
            }

            UpdateEdgeCollider();
        }


        private void UpdateEdgeCollider()
        {
            if (trailPoints.Count == 0)
            {
                edgeCollider.points = new Vector2[0];
                return;
            }

            Vector2[] colliderPoints = new Vector2[trailPoints.Count];
            for (int i = 0; i < trailPoints.Count; i++)
            {
                Vector3 pos = trailPoints[i].Position;
                colliderPoints[i] = new Vector2(pos.x, pos.y);
            }
            edgeCollider.points = colliderPoints;
        }

        public void CheckForShapes()
        {
            int[] expectedSidesArray = new int[] { 3, 4, 5 }; // Example: triangle, square, pentagon
            float[] angleThresholds = new float[] { 120f, 90f, 72f }; // Corresponding angle thresholds for shapes
            float angleMargin = 45f; // Further increased margin of error in degrees

            for (int j = 0; j < expectedSidesArray.Length; j++)
            {
                int expectedSides = expectedSidesArray[j];
                float idealAngle = angleThresholds[j];
                float lowerThreshold = idealAngle - angleMargin;
                float upperThreshold = idealAngle + angleMargin;

                List<Vector3> significantPoints = GetSignificantPoints(trailPoints.ConvertAll(p => p.Position), lowerThreshold, upperThreshold);

                Debug.Log("Significant points for expected sides " + expectedSides + ": " + significantPoints.Count);

                if (significantPoints.Count == expectedSides)
                {
                    float distanceToFirstPoint = Vector3.Distance(significantPoints[significantPoints.Count - 1], significantPoints[0]);
                    if (distanceToFirstPoint <= pointRadius)
                    {
                        // Shape formed, handle shape detection logic here
                        Debug.Log("Shape formed with " + expectedSides + " sides!");
                        GameManager.instance.SetMultiplier(expectedSides);

                        // Update lifetimes for points forming the shape
                        foreach (var point in trailPoints)
                        {
                            if (significantPoints.Contains(point.Position))
                            {
                                point.Lifetime = shapeLifetime;
                            }
                        }

                        return; // Exit once a valid shape is found
                    }
                }
            }

            // If no shape is detected, reset the multiplier
            GameManager.instance.SetMultiplier(1);
        }


        private List<Vector3> GetSignificantPoints(List<Vector3> points, float lowerThreshold, float upperThreshold)
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

                Debug.Log("Angle at point " + i + ": " + angle);

                if (angle >= lowerThreshold && angle <= upperThreshold)
                {
                    significantPoints.Add(currPoint); // Add significant point
                    Debug.Log("Significant point added at index " + i);
                }
            }

            significantPoints.Add(points[points.Count - 1]); // Add the last point

            return significantPoints;
        }
    }
}
