using System.Collections;
using UnityEngine;

public class StaffPickUp : PickUpHandler
{
    [SerializeField] private GameObject spawnBall;
    [SerializeField] private float attackInterval;
    private Transform _cam;
    private bool _hasAttacked;

    private void Awake()
    {
        _cam = Camera.main.transform;
    }

    private IEnumerator HandleAttackInterval()
    {
        _hasAttacked = true;
        yield return new WaitForSeconds(attackInterval);
        _hasAttacked = false;
    }
    public override void UsePickUp()
    {
        if (_hasAttacked) return;
        HandleAttack();
        StartCoroutine(HandleAttackInterval());
    }

    private void HandleAttack()
    {
        Instantiate(spawnBall, transform.position, _cam.rotation);
    }

    
}
