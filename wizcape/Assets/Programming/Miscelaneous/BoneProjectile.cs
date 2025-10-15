using System.Collections;
using UnityEngine;

public class BoneProjectile : MonoBehaviour
{
    [SerializeField] private DamageInstance damageInstance; // Damage to apply on hit
    [SerializeField] private float projectileSpeed = 10f; // Speed of the projectile
    [SerializeField] private ParticleSystem trailParticles; // Fire trail for fireballs
    [SerializeField] private GameObject explosionParticlesPrefab; // Explosion on impact
    [SerializeField] private float explosionRadius = 3f; // AOE radius for fireball
    [SerializeField] private float floatDuration = 1f; // Duration to float before moving
    [SerializeField] private float bobAmplitude = 0.2f; // Amplitude of bobbing motion
    [SerializeField] private float bobFrequency = 2f; // Frequency of bobbing motion

    [SerializeField] private float boneLifeTime = 10f; // Lifetime before self-destruction
    private Rigidbody rb; // For movement
    private Transform target; // Player target
    private Vector3 initialPosition; // Position at spawn for bobbing
    private bool isFloating = true; // Flag for floating phase

    private void Awake()
    {
        Destroy(gameObject, boneLifeTime);
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogWarning("BoneProjectile requires a Rigidbody component.", this);
            Destroy(gameObject);
            return;
        }
        rb.isKinematic = true; // Start kinematic to float in place
        rb.useGravity = false; // Disable gravity to prevent falling
        Debug.Log($"BoneProjectile: Initialized at {transform.position}, gravity disabled.");
    }

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
        initialPosition = transform.position;
            StartMoving(); // Fireballs move immediately
    }

    private IEnumerator FloatThenMove()
    {

        Debug.Log($"BoneProjectile: Floating at {transform.position} for {floatDuration} seconds.");

        float elapsed = 0f;
        while (elapsed < floatDuration)
        {
            if (target == null)
            {
                Debug.LogWarning("BoneProjectile: Target lost during floating, destroying.", this);
                Destroy(gameObject);
                yield break;
            }

            // Apply subtle bobbing motion
            float bobOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            transform.position = initialPosition + Vector3.up * bobOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        StartMoving();
    }

    private void StartMoving()
    {
        if (target == null)
        {
            Debug.LogWarning("BoneProjectile: Target lost when starting movement, destroying.", this);
            Destroy(gameObject);
            return;
        }

        isFloating = false;
        rb.isKinematic = false;
        Vector3 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = direction * projectileSpeed;
        Debug.Log($"BoneProjectile: Launching towards player at {target.position} with velocity {rb.linearVelocity} (speed: {projectileSpeed}).");

    }
    private void OnTriggerEnter(Collider other)
    {
        // Apply damage to IDamagable objects
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (damagable.damageLayer == DamageLayer.Friendly)
            {
                damageInstance.Execute(other.gameObject, DamageLayer.Enemy);
                Destroy(gameObject);
            }
        }

      
    }
}