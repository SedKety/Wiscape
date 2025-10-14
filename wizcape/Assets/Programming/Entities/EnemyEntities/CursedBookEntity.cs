using EditorAttributes;
using UnityEngine;
using UnityEngine.AI;

public class CursedBookEntity : EnemyEntity
{
    [GUIColor(GUIColor.Purple)][Header("Cursed Book Settings")][Tooltip("The cursed book's transform for hovering")][SerializeField] private Transform cursedBookGO;
    [Tooltip("Min/Max height the book hovers at")][SerializeField] private RandomFloatV2 hoverHeight;
    [Tooltip("The speed of the hover motion")][SerializeField] private float hoverFrequency = 1f;
    [Tooltip("The range or amplitude of the hover motion")][SerializeField] private float hoverRange = 0.5f;
    [Header("Projectile Settings")][Tooltip("The projectile prefab to shoot at the player")][SerializeField] private GameObject projectilePrefab;
    [Tooltip("Speed of the projectile")][SerializeField] private float projectileSpeed = 10f;
    [Tooltip("The point from which the projectile is spawned")][SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireBallLifetime;

    private Quaternion _originalRotation;
    private bool _isRotatingToPlayer;
    private bool _isRotatingBack;
    private float _rotationBackTimer;
    private Vector3 _directionToPlayer; // Store direction to player for reuse
    private const float ROTATION_BACK_DURATION = 1f; // Duration to rotate back to destination
    private const float ROTATION_TO_PLAYER_DURATION = 1f; // Duration for smooth rotation to player

    protected override void Awake()
    {
        cursedBookGO.localPosition = new Vector3(0f, hoverHeight.GetRandom(), 0f);
        base.Awake();
        _agent.acceleration = 20f; // Smoother acceleration
        _agent.stoppingDistance = 0.5f; // Prevent abrupt stops
        _agent.angularSpeed = rotationSpeed * 120f; // Faster rotation
        _isRotatingToPlayer = false;
        _isRotatingBack = false;
        _rotationBackTimer = 0f;
        _directionToPlayer = Vector3.zero;
    }

    private float _rotationToPlayerTimer; // New timer for rotating to player

    private void FixedUpdate()
    {
        float hoverY = hoverHeight.Last + Mathf.Sin(Time.time * hoverFrequency) * hoverRange;
        cursedBookGO.localPosition = new Vector3(0f, hoverY, 0f); // Update local position for hover

        // Check if the book is close to attacking (within striking range and attack delay almost expired)
        if (distanceToPlayer == Distance.WithinRange && _timeSinceLastHit >= attackDelay - 0.5f && !_isRotatingBack)
        {
            // Calculate direction to player once for this frame
            _directionToPlayer = (playerGO.transform.position - cursedBookGO.position).normalized;
            _directionToPlayer.y = 0; // Keep rotation in the horizontal plane

            if (!_isRotatingToPlayer)
            {
                // Store the original rotation and reset the timer
                _originalRotation = cursedBookGO.rotation;
                _isRotatingToPlayer = true;
                _rotationToPlayerTimer = 0f; // Reset the timer
            }

            // Increment the timer
            _rotationToPlayerTimer += Time.deltaTime;
            // Calculate interpolation factor
            float t = Mathf.Clamp01(_rotationToPlayerTimer / ROTATION_TO_PLAYER_DURATION);
            // Smoothly rotate towards the player
            Quaternion targetRotation = Quaternion.LookRotation(_directionToPlayer);
            cursedBookGO.rotation = Quaternion.Slerp(_originalRotation, targetRotation, t);
        }
        else if (_isRotatingBack)
        {
            // Smoothly rotate back to the NavMeshAgent's destination direction
            _rotationBackTimer += Time.deltaTime;
            float t = _rotationBackTimer / ROTATION_BACK_DURATION;
            Vector3 directionToDestination = (_agent.destination - cursedBookGO.position).normalized;
            directionToDestination.y = 0; // Keep rotation in the horizontal plane
            Quaternion destinationRotation = directionToDestination.sqrMagnitude > 0 ? Quaternion.LookRotation(directionToDestination) : _originalRotation;
            cursedBookGO.rotation = Quaternion.Slerp(_originalRotation, destinationRotation, t);
            if (_rotationBackTimer >= ROTATION_BACK_DURATION)
            {
                _isRotatingBack = false;
                _rotationBackTimer = 0f;
            }
        }
        else
        {
            _isRotatingToPlayer = false;
            _rotationToPlayerTimer = 0f; // Reset the timer when not rotating to player
        }
    }

    protected override MoveActions CalculateMoveAction()
    {
        // Always wander unless within striking range
        if (distanceToPlayer == Distance.WithinRange && CanStrikePlayer())
        {
            return MoveActions.Strike;
        }
        return MoveActions.Wander;
    }

    protected override void Strike(GameObject target)
    {
        _timeSinceLastHit = 0;
        _isRotatingToPlayer = false;
        _isRotatingBack = true;
        _rotationBackTimer = 0f;
        _originalRotation = cursedBookGO.rotation; // Store the rotation at the moment of striking

        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            // Use stored _directionToPlayer for projectile direction
            Vector3 projectileDirection = (target.transform.position - projectileSpawnPoint.position).normalized;
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth projectile motion
                rb.linearVelocity = projectileDirection * projectileSpeed;
            }
            FireballScript fireball = projectile.GetComponent<FireballScript>();
            if (fireball != null)
            {
                fireball.damageInstance = damageInstance;
                fireball.fireBallLifetime = fireBallLifetime;
            }
        }
    }

    protected override void Wander()
    {
        if (_agent == null) return;
        _wanderTimer -= Time.deltaTime;
        if (_wanderTimer <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            _wanderTimer = randomWanderPointInterval.GetRandom();
        }
        else if (Vector3.Distance(_agent.destination, transform.position) <= minDistanceToWanderPoint)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }
        }
    }
}