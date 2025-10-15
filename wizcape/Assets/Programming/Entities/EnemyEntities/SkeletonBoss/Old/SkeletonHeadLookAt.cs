using UnityEngine;

public class SkeletonHeadLookAt : MonoBehaviour
{
    [SerializeField] private bool lockYRotation = true; 
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private Transform head; 
    private Transform playerTransform;

    private void Start()
    {
        // Get the player transform at start
        GameObject player = PlayerLocation.GetPlayer();
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            // Try to find the player if not already set
            GameObject player = PlayerLocation.GetPlayer();
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                return; // No player found, skip update
            }
        }

        // Get the direction to the player
        Vector3 directionToPlayer = playerTransform.position - head.position;

        if (lockYRotation)
        {
            // Lock to Y-axis rotation (keep head level)
            directionToPlayer.y = 0f; // Ignore vertical difference
            if (directionToPlayer.sqrMagnitude < 0.01f)
            {
                return; // Avoid division by zero if direction is too small
            }

            // Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

            // Smoothly rotate towards the player
            head.rotation = Quaternion.Slerp(
                head.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        else
        {
            // Full 3D rotation to look at player
            head.LookAt(playerTransform);
        }
    }
}