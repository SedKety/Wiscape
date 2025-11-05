using UnityEngine;

public class SkeletonEntity : EnemyEntity
{
    protected override void Update()
    {
        base.Update();
        
        
    }

    protected override void Strike(GameObject target)
    {
        base.Strike(target);
        _animator.SetTrigger("Attack");

    }
} 