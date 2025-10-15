using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EditorAttributes;

public class BossController : EnemyEntity
{
    // Health Management (health inherited from EntityBase)
    [SerializeField] private int initialHealth = 1000; // Initial health of the boss
    [SerializeField] private float phase2HealthThreshold = 500f; // Health at which Phase 2 starts (50%)

    // Phases
    private bool isPhase2 = false; // Flag to indicate if in Phase 2

    // Movement
    [SerializeField] private float baseMoveSpeed = 2f; // Base movement speed
    [SerializeField] private Transform arenaCenter; // Center point of the arena for circular patrol
    [SerializeField] private float patrolRadius = 10f; // Radius of the circular patrol path
    private float patrolAngle = 0f; // Angle for circular movement calculation

    // Attacks
    [SerializeField] private float meleeRange = 5f; // Range for melee swipe
    [SerializeField] private GameObject boneProjectilePrefab; // Prefab for bone projectiles
    [SerializeField] private int minProjectiles = 3; // Min number of bone projectiles per summon
    [SerializeField] private int maxProjectiles = 5; // Max number of bone projectiles per summon
    [SerializeField] private float projectileSpeed = 10f; // Speed of projectiles
    [SerializeField] private float meleeChance = 0.2f; // 20% chance for melee if in range

    // Pillars (assign existing carts with PillarScript in Inspector)
    [SerializeField] private PillarScript[] pillars; // Existing pillars to trigger in Phase 2

    // Audio and VFX
    [SerializeField] private AudioSource audioSource; // For playing sounds
    [SerializeField] private AudioClip attackWhoosh; // Sound for attacks
    [SerializeField] private AudioClip hurtGrunt; // Sound for taking damage
    [SerializeField] private AudioClip roarSound; // Sound for phase transition
    [SerializeField] private AudioClip fireballLaunch; // Sound for fireball (played by carts)
    [SerializeField] private GameObject summonParticles; // Particles for bone summons
    [SerializeField] private GameObject explosionParticlesPrefab; // For fireball explosions

    // Coroutines and Flags
    private bool isAttacking = false; // Prevent attacks during transitions
    private bool isDead = false; // Flag for death state
    private Coroutine attackCoroutine; // Reference to the attack loop coroutine
    private List<Collider> activeBoneColliders = new List<Collider>(); // Track active bone colliders
    private Collider bossCollider; // Boss's collider for ignoring bone collisions

    protected override void Awake()
    {
        // Initialize EntityBase health
        health = initialHealth;

        // Initialize EnemyEntity fields
        attackRange = meleeRange;
        attackDelay = 5f; // Default attack cooldown
        detectionRange = patrolRadius * 2f;
        retreatDistance = patrolRadius * 0.5f;
        retreatTime = new RandomFloatV2(2f, 4f);
        randomWanderPointInterval = new RandomFloatV2(3f, 6f);
        moveSpeed = new RandomFloatV2(baseMoveSpeed, baseMoveSpeed);

        base.Awake(); // Initialize NavMeshAgent, animator, etc.

        // Disable NavMeshAgent rotation control
        if (_agent != null) _agent.updateRotation = false;

        // Initialize boss-specific components
        audioSource = GetComponent<AudioSource>();
        if (arenaCenter == null) arenaCenter = new GameObject("ArenaCenter").transform;

        // Get boss collider
        bossCollider = GetComponent<Collider>();

        // Update animator parameters
        _curMoveSpeed = moveSpeed.GetRandom();
        if (_animator != null)
        {
            _animator.SetFloat("MoveSpeed", moveSpeed.PercentageOfMax / 100f);
        }
    }

    protected override void Start()
    {
        base.Start(); // Set playerGO
        attackCoroutine = StartCoroutine(AttackLoop());
    }

    protected override void Update()
    {
        if (isDead) return;

        // Always face the player when in range, even during attacking
        LookAtPlayer();

        if (!isAttacking) base.Update();
    }

    private void LookAtPlayer()
    {
        if (playerGO == null)
        {
            Debug.LogWarning("BossController: playerGO is null, cannot rotate to face player.", this);
            return;
        }

        if (distanceToPlayer != Distance.OutOfRange || isAttacking)
        {
            Vector3 directionToPlayer = (playerGO.transform.position - transform.position).normalized;
            directionToPlayer.y = 0; // Keep rotation on Y-axis only
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = targetRotation;
            Debug.Log($"BossController: Rotating to face player at {playerGO.transform.position}, distanceToPlayer: {distanceToPlayer}");
        }
    }

    protected override void Fuzzify()
    {
        base.Fuzzify();
    }

    protected override Distance CalculateDistance()
    {
        return base.CalculateDistance();
    }

    protected override MoveActions CalculateMoveAction()
    {
        MoveActions ma = distanceToPlayer switch
        {
            Distance.WithinRange => CanStrikePlayer() ? MoveActions.Strike : MoveActions.Await,
            Distance.Close => MoveActions.Advance,
            Distance.Medium => MoveActions.Advance,
            Distance.Far => MoveActions.Run,
            Distance.OutOfRange => MoveActions.Wander,
            _ => MoveActions.Unknown
        };
        return ma;
    }

    protected override void Defuzzify()
    {
        if (isDead || isAttacking) return;

        switch (moveAction)
        {
            case MoveActions.Wander:
                Patrol();
                break;
            case MoveActions.Strike:
                if (Random.value <= meleeChance)
                {
                    Strike(playerGO);
                }
                else
                {
                    if (_animator != null) _animator.SetTrigger("Idle");
                }
                break;
            default:
                base.Defuzzify();
                break;
        }
    }

    private void Patrol()
    {
        patrolAngle += _curMoveSpeed * Time.deltaTime / patrolRadius;
        Vector3 targetPosition = arenaCenter.position + new Vector3(Mathf.Cos(patrolAngle) * patrolRadius, 0f, Mathf.Sin(patrolAngle) * patrolRadius);
        GoTo(targetPosition);
        if (_animator != null) _animator.SetTrigger("Idle");

        // Face movement direction when out of range
        if (playerGO != null && distanceToPlayer == Distance.OutOfRange && _agent != null && _agent.velocity.magnitude > 0.1f)
        {
            Vector3 moveDirection = _agent.velocity.normalized;
            moveDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(moveDirection);
            Debug.Log($"BossController: Patrolling, facing movement direction: {moveDirection}");
        }
    }

    protected override void Strike(GameObject target)
    {
        if (target != null)
        {
            Vector3 directionToPlayer = (target.transform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            Debug.Log($"BossController: Striking, facing player at {target.transform.position}");
        }
        base.Strike(target);
        if (_animator != null) _animator.SetTrigger("Attack");
        if (audioSource != null && attackWhoosh != null) audioSource.PlayOneShot(attackWhoosh);
    }

    public override void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical)
    {
        if (isDead) return;

        health -= intakeDamage;
        if (audioSource != null && hurtGrunt != null) audioSource.PlayOneShot(hurtGrunt);
        if (_animator != null) _animator.SetTrigger("Hurt");

        if (!isPhase2 && health <= phase2HealthThreshold)
        {
            StartCoroutine(PhaseTransition());
        }

        if (health <= 0)
        {
            StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator PhaseTransition()
    {
        isAttacking = true;
        if (attackCoroutine != null) StopCoroutine(attackCoroutine);

        if (_animator != null) _animator.SetTrigger("Roar");
        if (audioSource != null && roarSound != null) audioSource.PlayOneShot(roarSound);

        // Face the player during roar
        if (playerGO != null)
        {
            Vector3 directionToPlayer = (playerGO.transform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            Debug.Log($"BossController: Phase transition, facing player at {playerGO.transform.position}");
        }

        yield return new WaitForSeconds(2f);

        isPhase2 = true;

        // Trigger shooting and movement on all assigned pillars
        foreach (PillarScript pillar in pillars)
        {
            if (pillar != null)
            {
                pillar.StartShooting();
                Debug.Log($"BossController: Triggered StartShooting on pillar at {pillar.transform.position}.");
            }
            else
            {
                Debug.LogWarning("BossController: A pillar reference is null in the pillars array.");
            }
        }

        attackCoroutine = StartCoroutine(AttackLoop());
        isAttacking = false;
    }

    private IEnumerator DeathSequence()
    {
        isDead = true;
        if (_animator != null) _animator.SetTrigger("Death");

        Collider collider = GetComponent<Collider>();
        if (collider != null) collider.enabled = false;

        if (_agent != null) _agent.isStopped = true;

        UIScreenLogic.Instance.WinGame();
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }

    private IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackDelay);

            if (playerGO == null)
            {
                Debug.LogWarning("BossController: playerGO is null in AttackLoop, skipping projectile spawn.");
                continue;
            }

            if (moveAction == MoveActions.Strike && Random.value <= meleeChance) continue;

            // Face the player before spawning projectiles
            Vector3 directionToPlayer = (playerGO.transform.position - transform.position).normalized;
            directionToPlayer.y = 0;
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
            Debug.Log($"BossController: Spawning projectiles, facing player at {playerGO.transform.position}");

            // Stop movement during bone spawning in Phase 2
            bool wasMoving = false;
            if (isPhase2 && _agent != null && !_agent.isStopped)
            {
                wasMoving = true;
                _agent.isStopped = true;
                if (_animator != null) _animator.SetFloat("MoveSpeed", 0f);
                Debug.Log("BossController: Stopped movement for bone spawning in Phase 2.");
            }

            int numProjectiles = Random.Range(minProjectiles, maxProjectiles + 1);
            List<Collider> newBoneColliders = new List<Collider>();

            for (int i = 0; i < numProjectiles; i++)
            {
                // Spawn bones 2 units above the boss with increased offset
                Vector3 spawnPos = transform.position + Vector3.up * 2f + Random.insideUnitSphere * 1f;
                GameObject projectile = Instantiate(boneProjectilePrefab, spawnPos, Quaternion.identity);
                BoneProjectile bone = projectile.GetComponent<BoneProjectile>();
                Collider boneCollider = projectile.GetComponent<Collider>();

                if (bone != null)
                {
                    bone.SetTarget(playerGO.transform); // Signal BoneProjectile to float then move
                    Debug.Log($"BossController: Spawned bone at {spawnPos}, targeting player at {playerGO.transform.position}");
                }

                if (boneCollider != null)
                {
                    // Disable collisions with existing bones
                    foreach (Collider existingCollider in activeBoneColliders)
                    {
                        if (existingCollider != null)
                        {
                            Physics.IgnoreCollision(boneCollider, existingCollider);
                            Debug.Log($"BossController: Disabled collision between bone at {spawnPos} and existing bone.");
                        }
                    }
                    // Disable collision with boss
                    if (bossCollider != null)
                    {
                        Physics.IgnoreCollision(boneCollider, bossCollider);
                        Debug.Log($"BossController: Disabled collision between bone at {spawnPos} and boss.");
                    }
                    newBoneColliders.Add(boneCollider);
                }

                if (summonParticles != null) Instantiate(summonParticles, spawnPos, Quaternion.identity);
                yield return new WaitForSeconds(0.2f);
            }

            // Add new bone colliders to active list
            activeBoneColliders.AddRange(newBoneColliders);
            // Clean up destroyed colliders
            activeBoneColliders.RemoveAll(col => col == null);

            // Resume movement after bone spawning in Phase 2
            if (isPhase2 && wasMoving && _agent != null)
            {
                _agent.isStopped = false;
                if (_animator != null) _animator.SetFloat("MoveSpeed", moveSpeed.PercentageOfMax / 100f);
                Debug.Log("BossController: Resumed movement after bone spawning in Phase 2.");
            }
        }
    }

    public void PauseBoss(bool isPaused)
    {
        if (isPaused)
        {
            StopAllCoroutines();
            if (_animator != null) _animator.speed = 0f;
            if (_agent != null) _agent.isStopped = true;
        }
        else
        {
            attackCoroutine = StartCoroutine(AttackLoop());
            if (_animator != null) _animator.speed = 1f;
            if (_agent != null) _agent.isStopped = false;
        }
    }
}