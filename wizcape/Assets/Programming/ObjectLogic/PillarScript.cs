using System.Collections;
using UnityEngine;

public class PillarScript : MonoBehaviour
{
    [Header("Fireball Settings")]
    [Tooltip("Array of transforms where fireballs will spawn.")]
    [SerializeField] private Transform[] firePoints;
    [Tooltip("Fireball prefab to instantiate.")]
    [SerializeField] private GameObject fireball;
    [Tooltip("Minimum time interval between fireball shots.")]
    [SerializeField] private float minInterval = 1f;
    [Tooltip("Maximum time interval between fireball shots.")]
    [SerializeField] private float maxInterval = 3f;
    [Tooltip("Speed of the fireball.")]
    [SerializeField] private float fireballSpeed = 10f;

    [Header("Movement Settings")]
    [Tooltip("Waypoints for the pillar to move between.")]
    [SerializeField] private Transform[] waypoints;
    [Tooltip("Speed at which the pillar moves between waypoints.")]
    [SerializeField] private float moveSpeed = 2f;
    [Tooltip("Distance threshold to consider a waypoint reached.")]
    [SerializeField] private float waypointThreshold = 0.1f;

    private int currentWaypointIndex = 0;
    private bool isMoving = false;
    private bool isShooting = false;

    public void StartShooting()
    {
        if (!isShooting)
        {
            isShooting = true;
            isMoving = true;
            StartCoroutine(HandleFireballFiring());
            StartCoroutine(MoveBetweenWaypoints());
        }
    }

    public void StopShooting()
    {
        isShooting = false;
        isMoving = false;
    }

    private IEnumerator MoveBetweenWaypoints()
    {
        while (isMoving)
        {
            Transform targetWaypoint = waypoints[currentWaypointIndex];
            Vector3 startPosition = transform.position;
            float journeyLength = Vector3.Distance(startPosition, targetWaypoint.position);
            float startTime = Time.time;

            // Move towards the waypoint
            while (Vector3.Distance(transform.position, targetWaypoint.position) > waypointThreshold)
            {
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float fractionOfJourney = distanceCovered / journeyLength;
                transform.position = Vector3.Lerp(startPosition, targetWaypoint.position, fractionOfJourney);
                yield return null;
            }

            // Snap to waypoint position
            transform.position = targetWaypoint.position;

            // Move to the next waypoint
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            yield return null;
        }
    }

    private IEnumerator HandleFireballFiring()
    {
        while (isShooting)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            ExecuteFireball();
        }
    }

    private void ExecuteFireball()
    {
        foreach (Transform firePoint in firePoints)
        {
            GameObject f = Instantiate(fireball, firePoint.position, firePoint.rotation);
            Rigidbody rb = f.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = firePoint.forward * fireballSpeed;
            }
        }
    }
}