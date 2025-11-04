using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public float fireBallLifetime;
    public float fireballSpeed;
    public DamageInstance damageInstance;
    private DamageLayer _dl = DamageLayer.Enemy;

    private void Start()
    {
        _dl = DamageLayer.Enemy;
        Destroy(gameObject, fireBallLifetime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damageAble))
        {
            if (damageAble.damageLayer == _dl) return;
            print(damageInstance);
            damageInstance.Execute(other.gameObject, _dl);
            Destroy(gameObject);
            
        }

    }
}
