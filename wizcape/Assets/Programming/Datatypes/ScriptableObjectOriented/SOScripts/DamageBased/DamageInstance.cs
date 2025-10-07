using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DamageInstance", menuName = "ScriptableObjects/DamageBased/DamageInstance")]

public class DamageInstance : ScriptableObject
{
    [SerializeReference] private List<HitEffect> hitEffects;

    public void Execute(GameObject target, DamageLayer dl)
    {
        foreach (var effect in hitEffects)
        {
            effect.ApplyEffect(target,  dl);
        }
    }

}
