using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace KP
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private int maxHp = 100;
        [SerializeField] private int currentHp;

        public int MaxHp => maxHp;

        public int Hp
        {
            get => currentHp;
            private set
            {
                bool isDamage = value < currentHp;
                currentHp = Mathf.Clamp(value, 0, maxHp);
                if (isDamage)
                {
                    Damaged?.Invoke(currentHp);
                }
                else
                {
                    Healed?.Invoke(currentHp);
                }

                if(currentHp <= 0)
                {
                    Died?.Invoke();
                }
            }
        }

        public UnityEvent<int> Healed;
        public UnityEvent<int> Damaged;
        public UnityEvent Died;

        private void Awake() => currentHp = maxHp;

        public void Damage(int amount) => Hp -= amount;

        public void Heal(int amount) => Hp -= amount;

        public void HealFull() => Hp = maxHp;

        public void Kill() => Hp = 0;

        public void SetHealth(int amount) => Hp = amount;
    }
}