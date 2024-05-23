using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP
{
    public class PlayerAttack : MonoBehaviour
    {
        private CustomInputs input = null;
        private Rigidbody2D playerRB = null;
        private PlayerMovement playerMovement = null;
        [SerializeField] float attackDistance = 1f;
        [SerializeField] float attackDuration = 0.1f;

        private void Awake()
        {
            input = new CustomInputs();
            playerRB = GetComponent<Rigidbody2D>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        private void OnEnable()
        {
            input.Enable();
            input.Player.Attack.performed += OnAttackPerformed;
        }

        private void OnDisable()
        {
            input.Disable();
            input.Player.Attack.performed -= OnAttackPerformed;
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            StartCoroutine(Attack());
        }

        private IEnumerator Attack()
        {
            Vector2 originalPosition = playerRB.position;
            Vector2 attackDirection = playerMovement.moveVector.normalized; // Assume you have a way to get the current move direction
            Vector2 attackPosition = originalPosition + attackDirection * attackDistance;
            float elapsedTime = 0f;

            Debug.Log("Bam");

            while (elapsedTime < attackDuration)
            {
                playerRB.MovePosition(Vector2.Lerp(originalPosition, attackPosition, elapsedTime / attackDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            playerRB.MovePosition(attackPosition);
        }
    }
}
