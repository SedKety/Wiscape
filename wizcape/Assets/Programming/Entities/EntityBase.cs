using EditorAttributes;
using System.Collections;
using UnityEngine;

public class EntityBase : MonoBehaviour, IDamagable
{
    // Peak
    [GUIColor(GUIColor.Blue)]
    [Header("Entity Settings")]
    [SerializeField] protected int health; // The hitpoints of the entity

    [SerializeField] protected DamageLayer _damageLayer;
    [SerializeField] protected string painSound;
    public DamageLayer damageLayer { get => _damageLayer; set => _damageLayer = value; } // Implementation of the IDamagable interface


    

 
    public virtual void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical)
    {
        health -= intakeDamage;
        SoundTriggerScript.Instance.SetSound(painSound);
        if(health <= 0)
        {
            print($"Entity {gameObject.name}, has died");
            Destroy(gameObject);
        }
    }
}
