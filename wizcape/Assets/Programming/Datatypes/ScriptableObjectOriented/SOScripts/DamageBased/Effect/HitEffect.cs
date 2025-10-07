using System;
using System.Collections;
using UnityEngine;

[Serializable]
public abstract class HitEffect 
{
    [SerializeField] private string effectName;

    public virtual void ApplyEffect(GameObject target, DamageLayer dl)
    {
        Debug.Log($"Applying {effectName} effect to {target.name}");
    }
}

[Serializable]
public class DamageEffect : HitEffect
{
    public int damageAmount;
    public override void ApplyEffect(GameObject target, DamageLayer dl)
    {
        base.ApplyEffect(target, dl);

        if (target.TryGetComponent(out IDamagable damagable))
        {
            damagable.TakeDamage(damageAmount, dl);
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
        public override void ApplyEffect(GameObject target, DamageLayer dl)
        {
            base.ApplyEffect(target, dl);

            if (CoroutineStarter.coroutineHost == null)
            {
                Debug.LogError("CoroutineHost is null. Please ensure CoroutineStarter is initialized.");
                return;
            }
            CoroutineStarter.coroutineHost.StartCoroutine(BurnTarget(target, dl));
        }

        private IEnumerator BurnTarget(GameObject target, DamageLayer dl)
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
                damagable.TakeDamage(damageAmount, dl, DamageType.fire);
                curTick += burnTickInterval;
                yield return new WaitForSeconds(burnTickInterval);
            }
        }
    }
}
