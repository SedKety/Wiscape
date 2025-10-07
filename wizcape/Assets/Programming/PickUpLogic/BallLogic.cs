using UnityEngine;

public class BallLogic : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float destroyTime;
    [SerializeField] private DamageInstance staffHit;
    private Rigidbody _rb;
    private DamageLayer _damageLayer = DamageLayer.Friendly;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    private void Update()
    {
        _rb.linearVelocity = transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent(out IDamagable damageable) && !other.CompareTag("Player")) 
        {
            staffHit.Execute(other.gameObject, _damageLayer);
            Destroy(gameObject);
        }
    }


}
