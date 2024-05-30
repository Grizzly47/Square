using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KP
{
    public class PlayerAttack : MonoBehaviour
    {
        private CustomInputs input = null;
        private Rigidbody2D playerRB = null;
        private Camera mainCamera;
        [SerializeField] float attackDistance = 1f;
        [SerializeField] float attackDuration = 0.1f;

        private void Awake()
        {
            input = new CustomInputs();
            playerRB = GetComponent<Rigidbody2D>();
            mainCamera = Camera.main;
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
            Vector2 attackDirection = GetAttackDirection();
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

        private Vector2 GetAttackDirection()
        {
            Vector2 attackDirection = Vector2.zero;

            if (Gamepad.current != null && Gamepad.current.rightStick.ReadValue().sqrMagnitude > 0.1f)
            {
                // If using controller, get the right stick direction
                attackDirection = Gamepad.current.rightStick.ReadValue();
            }
            else
            {
                // If using keyboard and mouse, get the mouse position
                Vector2 mousePosition = Mouse.current.position.ReadValue();
                Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
                attackDirection = (worldMousePosition - playerRB.position).normalized;
            }

            return attackDirection.normalized;
        }
    }
}
