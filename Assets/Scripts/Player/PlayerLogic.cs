using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KP
{
    public class PlayerLogic : MonoBehaviour
    {
        private Health playerHealth;

        private void Awake()
        {
            playerHealth = GetComponent<Health>();
        }
    }
}