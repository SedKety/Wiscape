using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class HitEffect 
{
    [SerializeField] private string effectName;

    public virtual void ApplyEffect(GameObject target)
    {
        Debug.Log($"Applying {effectName} effect to {target.name}");
    }
}

[Serializable]
public class DamageEffect : HitEffect
{
    public int damageAmount;
    public override void ApplyEffect(GameObject target)
    {
        base.ApplyEffect(target);

        if (target.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damageAmount);
            Debug.Log($"Dealt: {damageAmount} damage to: {target.name}");
        }
        else
        {
            Debug.Log($"{target} has no IDamagable component.");

        }
    }

    [Serializable]
    public class FireEffect : HitEffect
    {
        public float burnDuration;
        public float burnTickInterval;

        public int damageAmount;
        public override void ApplyEffect(GameObject target)
        {
            base.ApplyEffect(target);

            if (CoroutineStarter.coroutineHost == null)
            {
                Debug.LogError("CoroutineHost is null. Please ensure CoroutineStarter is initialized.");
                return;
            }
            CoroutineStarter.coroutineHost.StartCoroutine(BurnTarget(target));
        }

        private IEnumerator BurnTarget(GameObject target)
        {
            var damagable = target.GetComponent<IDamagable>();
            if (damagable == null)
            {
                Debug.Log($"{target} has no IDamagable component.");
                yield return null;
            }

            var curTick = 0f;
            while (curTick < burnDuration)
            {
                damagable.TakeDamage(damageAmount, DamageType.fire);
                curTick += burnTickInterval;
                yield return new WaitForSeconds(burnTickInterval);
            }
        }
    }
}
