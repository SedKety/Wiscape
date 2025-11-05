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
    [SerializeField] private string painSound;
    public DamageLayer damageLayer { get => _damageLayer; set => _damageLayer = value; }

    public string soundEffect;
    // Implementation of the IDamagable interface

    private void Start()
    {
        StartCoroutine(RandomizeGrowling());
    }

    private IEnumerator RandomizeGrowling()
    {
        yield return null;
    }
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
