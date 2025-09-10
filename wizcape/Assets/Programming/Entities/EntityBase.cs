using UnityEngine;

public class EntityBase : MonoBehaviour, IDamagable
{
    public int health;
    public int attackPower;
    public void TakeDamage(int intakeDamage, DamageType dt = DamageType.physical)
    {
        health -= intakeDamage;
        if(health <= 0)
        {
            print($"Entity {gameObject.name}, has died");
            Destroy(gameObject);
        }
    }
}
