using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public float fireBallLifetime;
    public float fireballSpeed;
    public DamageInstance damageInstance;
    private DamageLayer _dl = DamageLayer.Enemy;

    private void Start()
    {
        Destroy(gameObject, fireBallLifetime);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamagable damageAble))
        {
            if (damageAble.damageLayer == _dl) return; //See if its a fellow enemy
            damageInstance.Execute(other.gameObject, _dl);
        }
        Destroy(gameObject);
    }
}
