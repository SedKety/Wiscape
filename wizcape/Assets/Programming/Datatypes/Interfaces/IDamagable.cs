using System;
using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical);
}

[Flags]
public enum DamageLayer : byte
{
    Friendly = 1,
    Enemy = 2,
    Prop = 4
}

