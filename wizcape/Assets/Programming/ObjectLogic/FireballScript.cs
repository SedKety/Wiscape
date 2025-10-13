using UnityEngine;

public class FireballScript : MonoBehaviour
{
    [SerializeField] private float fireballSpeed;
    [SerializeField] private DamageInstance damageInstance;
    private DamageLayer _dl = DamageLayer.Enemy;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }
    private void Update()
    {
        transform.Translate(Vector3.forward * fireballSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            if (other.TryGetComponent(out IDamagable damageAble))
            {
                damageInstance.Execute(other.gameObject, _dl);
            }

            else
            {
                damageInstance.Execute(other.transform.root.gameObject, _dl);
            }
            
        }

        Destroy(gameObject);
    }
}
