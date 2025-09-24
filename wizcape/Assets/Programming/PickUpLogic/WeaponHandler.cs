using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class WeaponHandler : PickUpHandler
{
    public int attackPower;
    public Animator attackAnimator;
    public float weaponDelay;
    public bool isAttacking;
    [SerializeField] private DamageInstance handslap;
    [SerializeField] private GameObject overworldWeapon;



    public override void UsePickUp() //Is triggered by the attack input.
    {
        if (isAttacking) return;
        attackAnimator.SetTrigger("Attack");
        AttackHandling();
        StartCoroutine(AttackDelay());

    }

    //Handles the attacking 
    protected virtual void AttackHandling()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxDistance: 5f))
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

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, 5f))
        {

        }
    }
    
    
}
