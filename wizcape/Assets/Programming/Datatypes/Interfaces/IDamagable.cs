using UnityEngine;

public interface IDamagable
{

    public void TakeDamage(int intakeDamage, DamageType dt = DamageType.physical);
}
