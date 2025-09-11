using System.Collections.Generic;
using UnityEngine;

public class TestMice : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int rays;
    [SerializeField] private float rayDistance;

    [SerializeField] private Transform rayOrigin;

    [SerializeField] private bool debugMode;

    [SerializeField] private float fov = 180f; // Field of view in degrees
    [SerializeField] private float angleStep; // For symmetrical results

    [SerializeField] private Vector3 currentDirection;

    [SerializeField] private float rotationSpeed;
    private void Awake()
    {
        angleStep = fov / (rays - 1);
        currentDirection = rayOrigin.forward;
    }
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector3 steering = Vector3.zero;
        int hitCount = 0;

        // Cast rays
        RaycastHit[] hits = FireRays();

        for (int i = 0; i < hits.Length; i++)
        {
            float angle = -fov / 2f + i * angleStep;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;

            if (hits[i].collider != null)
            {
                // Push away from obstacle (inverse of ray direction)
                steering -= direction;
                hitCount++;
            }
        }

        Vector3 desiredDirection;

        if (hitCount > 0)
        {
            // Average repulsion from all obstacles
            desiredDirection = (currentDirection + steering).normalized;
        }
        else
        {
            // No obstacles, keep moving forward
            desiredDirection = currentDirection;
        }

        currentDirection = Vector3.Slerp(currentDirection, desiredDirection, Time.deltaTime * 5f);

        // Move
        transform.Translate(currentDirection * moveSpeed * Time.deltaTime, Space.World);

        // Rotate to face movement
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }

    private RaycastHit[] FireRays()
    {
        if (rayOrigin == null) return null;

        var hits = new RaycastHit[rays];


        for (int i = 0; i < rays; i++)
        {
            float angle = -fov / 2f + i * angleStep; // start left, sweep right
            Vector3 direction = Quaternion.Euler(0, angle, 0) * rayOrigin.forward;
            Physics.Raycast(rayOrigin.position, direction, out RaycastHit hit, rayDistance);
            hits[i] = hit;
        }
        return hits;
    }
    private void OnDrawGizmos()
    {
        if(!debugMode) return;
        if (rayOrigin == null) return;

        Gizmos.color = Color.red;


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
