using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyEntity : EntityBase
{
    [Header("Enemy Settings")]
    [SerializeField] protected GameObject playerGO; // The player game object, used for distance calculations

    [Header("Combat variables")]
    [SerializeField] protected DamageInstance damageInstance; // The effects that the enemy will apply to other entities
    [SerializeField] protected float attackRange; // From how far away the enemy is able to hit other entities

    [Header("Movement variables")]
    [SerializeField] protected float detectionRange; // From how far away the enemy is able to detect other entities
    protected float _curMoveSpeed; // The current movespeed set by the method that invokes GoTo
    [SerializeField] protected float moveSpeed; // How fast the enemy moves
    [SerializeField] protected float runningSpeed; // How fast the enemy moves when running
    protected Vector3 currentDirection; // Currnt direction the enemy is moving in

    [Header("Raycasting settings")]
    [Min(3)]
    [SerializeField] protected int rays = 5; // Amount of raycasts sent out for collision checks
    [SerializeField] protected float rayDistance = 3f; // How far the raycasts reach
    [SerializeField] protected Transform rayOrigin; // The origin of the ray
    [SerializeField] protected float fov = 180f; // Field of view in degrees
    [SerializeField] protected float rotationSpeed = 5f; // How fast the enemy rotates to face the movement direction
    protected float angleStep; // For symmetrical results

    [Tooltip("Whether or not to view the raycasts with gizmos")]
    [SerializeField] protected bool debugMode;

    [SerializeField] protected Rigidbody rb;

    private Vector3 fp = Vector3.zero; // For debug gizmos
    private Vector3 futurePos { get { return fp + Vector3.forward; } set { fp = value; } }

    protected void Awake()
    {
        _curMoveSpeed = moveSpeed;
        angleStep = fov / (rays - 1);
        currentDirection = rayOrigin.forward;

        if (rb == null) rb = GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    protected virtual void Update()
    {
        
    }

    protected virtual void MoveInDirection(Vector3 desiredDirection)
    {
        desiredDirection.y = 0;
        currentDirection = Vector3.Slerp(currentDirection, desiredDirection, Time.deltaTime * 5f).normalized;

        Vector3 moveXZ = new Vector3(currentDirection.x, 0, currentDirection.z);
        Vector3 targetPos = rb.position + moveXZ * _curMoveSpeed * Time.deltaTime;
        futurePos = targetPos; // Just for debug

        rb.MovePosition(targetPos);

        if (moveXZ != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveXZ);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * rotationSpeed));
        }
    }

    protected Vector3 CalculateAvoidance()
    {
        if (rayOrigin == null) return Vector3.zero;

        Vector3 steering = Vector3.zero;
        int hitCount = 0;

        RaycastHit[] hits = FireRays();

        for (int i = 0; i < hits.Length; i++)
        {
            float angle = -fov * .5f + i * angleStep;
            Vector3 rayDir = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;

            if (hits[i].collider != null)
            {
                rayDir.y = 0;

                // Steer away from obstacle
                Vector3 awayFromObstacle = Vector3.Cross(Vector3.up, rayDir).normalized;

                float weight = 1f - (hits[i].distance / rayDistance);
                steering += awayFromObstacle * weight;
                hitCount++;
            }
        }

        return hitCount > 0 ? steering.normalized : Vector3.zero;
    }

    protected void Wander()
    {
        if (rayOrigin == null) return;

        Vector3 avoidance = CalculateAvoidance();
        Vector3 forwardBias = currentDirection.normalized;

        Vector3 desiredDirection = avoidance != Vector3.zero
            ? (avoidance * 0.7f + forwardBias * 0.3f).normalized
            : forwardBias;

        MoveInDirection(desiredDirection);
    }

    protected virtual void GoTo(Vector3 targetPosition)
    {
        if (rayOrigin == null) return;

        Vector3 avoidance = CalculateAvoidance();
        Vector3 toTarget = (targetPosition - transform.position);
        toTarget.y = 0;
        toTarget.Normalize();

        Vector3 desiredDirection = avoidance != Vector3.zero
            ? (toTarget + avoidance).normalized
            : toTarget;

        MoveInDirection(desiredDirection);
    }

    protected RaycastHit[] FireRays()
    {
        if (rayOrigin == null) return new RaycastHit[rays];

        var hits = new RaycastHit[rays];
        for (int i = 0; i < rays; i++)
        {
            float angle = -fov / 2f + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;
            Physics.Raycast(rayOrigin.position, direction, out hits[i], rayDistance);
        }
        return hits;
    }

    protected virtual void OnDrawGizmos()
    {
        if (!debugMode || rayOrigin == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(futurePos, .5f);

        for (int i = 0; i < rays; i++)
        {
            float angle = -fov / 2f + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;
            Physics.Raycast(rayOrigin.position, direction, out RaycastHit hit, rayDistance);

            Gizmos.color = hit.collider != null ? Color.green : Color.red;
            Gizmos.DrawRay(rayOrigin.position, direction * rayDistance);


        }
    }
}