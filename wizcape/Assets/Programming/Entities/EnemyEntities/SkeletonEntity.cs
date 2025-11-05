using System.Collections;
using UnityEngine;

public class SkeletonEntity : EnemyEntity
{
    protected override void Update()
    {
        base.Update();
        _animator.SetTrigger("IsStriking");
    }
} 