using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackSystem : MonoBehaviour
{
    [SerializeField] private Animator _handAnimator;
    private bool _isAttacking;

    [SerializeField] private DamageInstance handslap;

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
        if(Physics.Raycast(ray: new Ray(transform.position, transform.forward), out RaycastHit hitInfo, maxDistance: 20f))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamagable damagable))
            {
                handslap.Execute(hitInfo.collider.gameObject); 
            }
        }
        yield return new WaitForSeconds(1);
        _isAttacking = false;
    }   
}
