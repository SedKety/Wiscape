using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using EditorAttributes;

public enum MoveActions
{
    Advance, // Move close
    Run, // Advance, but faster
    Await, // Stay close to the player and await the time till next attack
    Retreat, // Move back
    Wander, // Walk around
    StrafeLeft, // Dodge left
    StrafeRight, // Dodge right
    Idle, // Stand still
    Strike, // Attack the player
    Unknown,
}

public enum Distance
{
    OutOfRange, // Out of the detection range
    Far, // Far away from the player, half of the detection range
    Medium, // Medium distance, one third of the detection range
    Close, // Close to the player, one quarter of the detection range
    WithinRange, // Within striking distance
}

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyEntity : EntityBase
{
    [GUIColor(GUIColor.Red)]
    [Header("Enemy Entity Settings")]
    [SerializeField] protected GameObject playerGO; // The player game object, used for distance calculations

    [Header("Combat variables")]
    [SerializeField] protected DamageInstance damageInstance; // The effects that the enemy will apply to other entities
    [SerializeField] protected float attackRange; // From how far away the enemy is able to hit other entities
    [SerializeField] protected float attackDelay = 0; // Time till the next attack action can be performed
    protected float _timeSinceLastHit = 0; // Time since the enemy last attacked something
    protected DamageLayer _damageLayer = DamageLayer.Enemy;

    [Header("Movement variables")]
    [SerializeField] protected float detectionRange; // From how far away the enemy is able to detect other entities
    [SerializeField] protected float retreatDistance; // Multiplier used to calculate how far away the enemy retreats
    [SerializeField] protected RandomFloatV2 retreatTime; // Time the enemy will retreat for
    protected float _curRetreatTime;
    protected float _timeSpentRetreating; // Time spent retreating
    protected float _pDistance; // Distance to player

    [Header("Movement speeds")]
    [SerializeField] protected RandomFloatV2 moveSpeed; // How fast the enemy normally moves
    [SerializeField] protected RandomFloatV2 runningSpeed; // How fast the enemy moves when running

#if UNITY_EDITOR
    [SerializeField, ReadOnly] protected float debugRunningSpeed; // To show the running speed in the inspector
    [SerializeField, ReadOnly] protected float debugMoveSpeed; // To show the normal movement speed in the inspector
#endif

    [SerializeField] protected float rotationSpeed; // How fast the enemy rotates to face the movement direction

    [Header("Wandering variables")]
    [SerializeField] protected float wanderRadius; // From how far away wander points can be chosen
    [SerializeField] protected RandomFloatV2 randomWanderPointInterval; // Time between wander destination changes
    protected float _wanderTimer; // For how long the enemy has been wandering toward the current destination

    [Header("Fuzzified variables")]
    [SerializeField] protected Distance distanceToPlayer = Distance.OutOfRange; // Fuzzified version of _pDistance
    [SerializeField] protected MoveActions moveAction = MoveActions.Wander; // Fuzzified result of distance and other variables

    protected float minDistanceToWanderPoint = .5f; // How far away the current point has to be for the enemy to pick a new one
    protected float _curMoveSpeed; // The current movespeed set by the method that invokes GoTo
    protected Vector3 _currentDirection; // Current direction the enemy is moving in
    protected NavMeshAgent _agent;

    [SerializeField] protected Animator _animator;
    protected virtual void Awake()
    {
        _curMoveSpeed = moveSpeed.GetRandom();
        runningSpeed.GetRandom();
#if UNITY_EDITOR
        debugMoveSpeed = _curMoveSpeed;
        debugRunningSpeed = runningSpeed.Last;
#endif
        _wanderTimer = randomWanderPointInterval.GetRandom(); // To pre-calculate how long it takes for the entity to find a new point
        _curRetreatTime = retreatTime.GetRandom();
        _timeSinceLastHit = attackDelay; // So the enemy can attack immediately
        _timeSpentRetreating = _curRetreatTime; // So the enemy doesn't retreat immediately

        _currentDirection = transform.forward;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _curMoveSpeed;
        _agent.angularSpeed = rotationSpeed * 60f; // Convert to degrees per second for NavMeshAgent
        _agent.acceleration = 8f; // How fast the _agent's speed increases from standing still to max velocity
        _agent.stoppingDistance = 0.1f; // How close the _agent needs to be to its destination for it to stop moving
        _agent.autoBraking = true; // To prevent walking past the desired destination
        _agent.updateRotation = true; // Let the _agent handle rotation
        _agent.updatePosition = true; // Let the _agent handle position

        _animator = _animator == null ? GetComponentInChildren<Animator>() : _animator;

        // So every enemy moves slightly different(Hier werdt door Tamas op geklaagd)
        _animator.SetFloat("MoveSpeed", (_curMoveSpeed - 0) / (moveSpeed.max - 0));
        if (runningSpeed.Last <= 0) return;
        _animator.SetFloat("RunningSpeed", (runningSpeed.Last - 0) / (runningSpeed.max - 0));
    }
    

    protected virtual void Start()
    {
        playerGO = PlayerLocation.GetPlayer();
    }

    protected virtual void Update()
    {
        _agent.speed = _curMoveSpeed;
        _timeSinceLastHit += Time.deltaTime;
        _timeSpentRetreating += Time.deltaTime;

        if (_timeSpentRetreating >= _curRetreatTime) { _curRetreatTime = retreatTime.GetRandom(); }

        Fuzzify();
        Defuzzify();
    }

    protected virtual void Fuzzify()
    {
        distanceToPlayer = CalculateDistance();
        moveAction = CalculateMoveAction();
    }

    protected virtual Distance CalculateDistance()
    {
        _pDistance = Vector3.Distance(transform.position, playerGO.transform.position);
        Distance d = _pDistance switch
        {
            var p when p <= attackRange => Distance.WithinRange,
            var p when p <= detectionRange / 4 => Distance.Close,
            var p when p <= detectionRange / 3 => Distance.Medium,
            var p when p <= detectionRange / 2 => Distance.Far,
            _ => Distance.OutOfRange
        };
        return d;
    }

    protected virtual bool CanStrikePlayer()
    {
        if (attackDelay >= _timeSinceLastHit)
        {
            return false;
        }
        if (_pDistance <= attackRange)
        {
            return true;
        }
        return false;
    }

    protected virtual MoveActions CalculateMoveAction()
    {
        MoveActions ma = distanceToPlayer switch
        {
            var d when d == Distance.WithinRange => CanStrikePlayer() ? MoveActions.Strike : MoveActions.Retreat,
            var d when d == Distance.Close => _timeSinceLastHit >= attackDelay ? MoveActions.Advance : MoveActions.Await,
            var d when d == Distance.Medium => MoveActions.Advance,
            var d when d == Distance.Far => MoveActions.Run,
            var d when d == Distance.OutOfRange => MoveActions.Wander,
            _ => MoveActions.Unknown
        };

        if (distanceToPlayer != Distance.Far && moveAction == MoveActions.Retreat && _timeSpentRetreating < _curRetreatTime)
        {
            ma = MoveActions.Retreat;
        }
        return ma;
    }

    protected virtual void Defuzzify()
    {
        if (runningSpeed.Last > 0)
        {
            _animator.SetBool("IsRunning", false);
        }
        float _timeSpentRetreatingPrev = _timeSpentRetreating;
        _timeSpentRetreating = 0;
        switch (moveAction)
        {
            case MoveActions.Advance:
                Advance();
                break;
            case MoveActions.Run:
                Run();
                break;
            case MoveActions.Await:
                Await();
                break;
            case MoveActions.Retreat:
                _timeSpentRetreating = _timeSpentRetreatingPrev;
                Retreat();
                break;
            case MoveActions.Wander:
                Wander();
                break;
            case MoveActions.StrafeLeft:
                StrafeLeft();
                break;
            case MoveActions.StrafeRight:
                StrafeRight();
                break;
            case MoveActions.Idle:
                Idle();
                break;
            case MoveActions.Strike:
                Strike(playerGO);
                break;
            default:
                break;
        }
    }

    protected virtual void Run()
    {
        if (runningSpeed.Last > 0)
        {
            _animator.SetBool("IsRunning", true);
        }
        _curMoveSpeed = runningSpeed.Last;
        GoTo(playerGO.transform.position);
    }

    protected virtual void Advance()
    {
        _curMoveSpeed = moveSpeed.Last;
        GoTo(playerGO.transform.position);
    }

    protected virtual void Await()
    {
        // Do nothing, stay in place
    }

    protected virtual void Retreat()
    {
        if (runningSpeed.Last > 0)
        {
            _animator.SetBool("IsRunning", true);
        }
        _curMoveSpeed = runningSpeed.Last;
        Vector3 directionAwayFromPlayer = (transform.position - playerGO.transform.position).normalized;
        float randomAngle = Random.Range(-90f, 90f);
        Vector3 retreatDir = Quaternion.Euler(0, randomAngle, 0) * directionAwayFromPlayer;
        Vector3 retreatPos = transform.position + retreatDir * retreatDistance;
        retreatPos.y = transform.position.y;
        GoTo(retreatPos);
    }

    protected virtual void StrafeLeft()
    {
        // Placeholder for strafe left behavior
    }

    protected virtual void StrafeRight()
    {
        // Placeholder for strafe right behavior
    }

    protected virtual void Idle()
    {
        // Stand still
    }

    protected virtual void Strike(GameObject target)
    {
        _timeSinceLastHit = 0;
        damageInstance.Execute(target, _damageLayer);
    }

    protected virtual void MoveInDirection(Vector3 desiredDirection)
    {
        if (_agent == null) return;
        desiredDirection.y = 0;
        _currentDirection = Vector3.Slerp(_currentDirection, desiredDirection, Time.deltaTime * 5f).normalized;
        Vector3 targetPos = transform.position + _currentDirection * _curMoveSpeed * Time.deltaTime;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }

    protected virtual void Wander()
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

    protected virtual void GoTo(Vector3 targetPosition)
    {
        if (_agent == null) return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
        }
    }
}