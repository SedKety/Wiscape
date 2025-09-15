using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public int attackPower;
    public Animator attackAnimator;
    public float weaponDelay;
    public bool isAttacking;
    [SerializeField] private DamageInstance handslap;
    [SerializeField] private GameObject overworldWeapon;



    public virtual void UseAttack()
    {
        if (isAttacking) return;
        attackAnimator.SetTrigger("Attack");
        AttackHandling();
        StartCoroutine(AttackDelay());

    }

    protected virtual void AttackHandling()
    {
        if (Physics.Raycast(ray: new Ray(transform.position, transform.forward), out RaycastHit hitInfo, maxDistance: 20f))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamagable damagable))
            {
                handslap.Execute(hitInfo.collider.gameObject);
            }
        }
    }

    private IEnumerator AttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(weaponDelay);
        isAttacking = false;
    }
    
    public GameObject ReturnOverworldWeapon()
    {
        return overworldWeapon;
    }
}
