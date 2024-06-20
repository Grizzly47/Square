using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class TrailPoint
    {
        public Vector3 Position { get; set; }
        public float Lifetime { get; set; }
        public float InitialLifetime { get; private set; }

        public TrailPoint(Vector3 position, float lifetime)
        {
            Position = position;
            Lifetime = lifetime;
            InitialLifetime = lifetime;
        }

        public float GetAlpha()
        {
            return Lifetime / InitialLifetime;
        }
    }
}