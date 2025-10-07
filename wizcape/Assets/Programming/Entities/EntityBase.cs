using EditorAttributes;
using UnityEngine;

public class EntityBase : MonoBehaviour, IDamagable
{
    // Peak
    [GUIColor(GUIColor.Blue)]
    [Header("Entity Settings")]
    [SerializeField] protected int health; // The hitpoints of the entity

    // Implementation of the IDamagable interface
    public virtual void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical)
    {
        health -= intakeDamage;
        if(health <= 0)
        {
            print($"Entity {gameObject.name}, has died");
            Destroy(gameObject);
        }
    }
}
