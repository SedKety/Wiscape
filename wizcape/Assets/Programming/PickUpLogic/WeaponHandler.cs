using System.Collections;
using UnityEngine;

public class WeaponHandler : PickUpHandler
{
    public int attackPower;
    public Animator attackAnimator;
    public float weaponDelay;
    public bool isAttacking;
    public string soundEffectName;
    [SerializeField] private DamageInstance handslap;
    [SerializeField] private GameObject overworldWeapon;
    [SerializeField] private LayerMask enemyLayer;
    private DamageLayer _damageLayer = DamageLayer.Friendly;


    public override void UsePickUp() //Is triggered by the attack input.
    {
        if (isAttacking) return;
        attackAnimator.SetTrigger("Attack");
        SoundTriggerScript.Instance.SetSound(soundEffectName);
        AttackHandling();
        StartCoroutine(AttackDelay());

    }

    //Handles the attacking 
    protected virtual void AttackHandling()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, maxDistance: 5f, enemyLayer))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamagable damagable))
            {
                if (hitInfo.transform.GetComponent<HitIndicatorManagement>().CheckIfBeingHit()) return;
                hitInfo.transform.GetComponent<HitIndicatorManagement>().StartHitIndicator();
                handslap.Execute(hitInfo.collider.gameObject, _damageLayer);
            }
        }
    }

    private IEnumerator AttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(weaponDelay);
        isAttacking = false;
    }
    
    
}
