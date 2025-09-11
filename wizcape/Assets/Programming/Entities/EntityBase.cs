using UnityEngine;

public class EntityBase : MonoBehaviour, IDamagable
{
    [Header("Entity Settings")]
    public int health; // The hitpoints of the entity

    // Implementation of the IDamagable interface
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
