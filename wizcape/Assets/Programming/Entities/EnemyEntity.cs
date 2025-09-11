using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : EntityBase
{
    [Header("Enemy Settings")]
    [SerializeField] protected GameObject playerGO; // the player game object

    [Header("Combat variables")]
    [SerializeField] protected DamageInstance damageInstance; // The effects that the enemy will apply to other entities
    [SerializeField] protected float attackRange; // From how far away the enemy is able to hit other entities

    [Header("Movement variables")]
    [SerializeField] protected float detectionRange; // From how far away the enemy is able to detect other entities
    protected float _curMoveSpeed;
    [SerializeField] protected float moveSpeed; // How fast the enemy moves
    [SerializeField] protected float runningSpeed; // How fast the enemy moves when running
    protected Vector3 currentDirection; // Currnt direction the enemy is moving in

    [Header("Raycasting settings")]
    [Range(5, 100)]
    [SerializeField] protected int rays = 5; // Amount of raycasts sent out for collision checks
    [SerializeField] protected float rayDistance = 3f; // How far the raycasts reach
    [SerializeField] protected Transform rayOrigin; // The origin of the ray
    [SerializeField] protected float fov = 180f; // Field of view in degrees
    [SerializeField] protected float rotationSpeed = 5f; // How fast the enemy rotates to face the movement direction
    protected float angleStep; // For symmetrical results

    [Tooltip("Whether or not to view the raycasts with gizmos")]
    [SerializeField] private bool debugMode;

    protected void Awake()
    {
        _curMoveSpeed = moveSpeed;
        angleStep = fov / (rays - 1);
        currentDirection = rayOrigin.forward;
    }
    protected virtual void Update()
    {

    }

    [Tooltip("Moves toward an direction, no collision checks in itself")]
    protected virtual void MoveInDirection(Vector3 desiredDirection)
    {
        desiredDirection.y = 0;
        currentDirection = Vector3.Slerp(currentDirection, desiredDirection, Time.deltaTime * 5f).normalized;

        Vector3 moveXZ = new Vector3(currentDirection.x, 0, currentDirection.z);
        transform.Translate(moveXZ * _curMoveSpeed * Time.deltaTime, Space.World);

        if (moveXZ != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveXZ);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }

    [Tooltip("Calculates where to steer to based on raycasts")]
    protected Vector3 CalculateAvoidance()
    {
        if (rayOrigin == null) return Vector3.zero;

        Vector3 steering = Vector3.zero;
        int hitCount = 0;

        RaycastHit[] hits = FireRays();

        for (int i = 0; i < hits.Length; i++)
        {
            float angle = -fov / 2f + i * angleStep;
            Vector3 rayDir = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;

            if (hits[i].collider != null)
            {
                rayDir.y = 0;

                // Steers away from the obstacle
                Vector3 awayFromObstacle = Vector3.Cross(Vector3.up, rayDir).normalized;

                // Weight avoidance by proximity (steer away steeper fron closer obstacles)
                float weight = 1f - (hits[i].distance / rayDistance);

                steering += awayFromObstacle * weight;
                hitCount++;
            }
        }

        return hitCount > 0 ? steering.normalized : Vector3.zero;
    }



    [Tooltip("Wanders around, avoiding obstacles")]
    protected void Wander()
    {
        if (rayOrigin == null) return;

        Vector3 avoidance = CalculateAvoidance();
        Vector3 forwardBias = currentDirection.normalized;

        Vector3 desiredDirection;

        if (avoidance != Vector3.zero)
        {
            // Blend avoidance with some forward movement
            desiredDirection = (avoidance * 0.7f + forwardBias * 0.3f).normalized;
        }
        else
        {
            // No obstacles, go straight
            desiredDirection = forwardBias;
        }

        MoveInDirection(desiredDirection);
    }




    [Tooltip("Finds obstacles, and moves to a position steering away from other obstacles")]
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




    // Check where nearby colliders are
    protected RaycastHit[] FireRays()
    {
        if (rayOrigin == null) return new RaycastHit[rays];

        var hits = new RaycastHit[rays];
        for (int i = 0; i < rays; i++)
        {
            float angle = -fov / 2f + i * angleStep; // From left to right
            Vector3 direction = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;
            Physics.Raycast(rayOrigin.position, direction, out hits[i], rayDistance);
        }
        return hits;
    }

    // To see where the rays and such go(Only works if debugmode is set to true)
    protected virtual void OnDrawGizmos()
    {
        if (!debugMode || rayOrigin == null) return;

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
