using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private Transform hand;
    private Animator _handAnimator;
    private bool _isAttacking;

    private void Awake()
    {
        _handAnimator = hand.GetComponent<Animator>();
    }

    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed && !_isAttacking)
        {
            StartCoroutine(HandleAttack());
        }
    }

    private IEnumerator HandleAttack()
    {
        _isAttacking = true;
        _handAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(1);
        _isAttacking = false;
    }   
}
