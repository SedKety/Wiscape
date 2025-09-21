using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyEntity : EntityBase
{
    [Header("Enemy Settings")]
    [SerializeField] protected GameObject playerGO; // The player game object, used for distance calculations

    [Header("Combat variables")]
    [SerializeField] protected DamageInstance damageInstance; // The effects that the enemy will apply to other entities
    [SerializeField] protected float attackRange; // From how far away the enemy is able to hit other entities

    [Header("Movement variables")]
    [SerializeField] protected float detectionRange; // From how far away the enemy is able to detect other entities

    [Header("Movement speeds")]
    [SerializeField] protected RandomFloatV2 moveSpeed; // How fast the enemy moves
    [SerializeField] protected RandomFloatV2 runningSpeed; // How fast the enemy moves when running

    [Space]

    [SerializeField] protected float rotationSpeed; // How fast the enemy rotates to face the movement direction

    [Header("Wandering variables")]
    [SerializeField] protected float wanderRadius; // From how far away wander points can be chosen
    [SerializeField] protected RandomFloatV2 randomWanderPointInterval; // Time between wander destination changes
    private float wanderTimer; // For how long the enemy has been wandering toward the current destination

    protected float minDistanceToWanderPoint = .5f; // How far away the current point has to be for the enemy to pick a new one
    protected float _curMoveSpeed; // The current movespeed set by the method that invokes GoTo
    protected Vector3 currentDirection; // Current direction the enemy is moving in

    protected NavMeshAgent agent;

    protected void Awake()
    {
        
        _curMoveSpeed = moveSpeed.GetRandom();
        runningSpeed.GetRandom(); // To pre-calculate the runningspeed of this entity
        wanderTimer = randomWanderPointInterval.GetRandom(); // To pre-calculate how long it takes for the entity to find a new point

        currentDirection = transform.forward;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = _curMoveSpeed;
        agent.angularSpeed = rotationSpeed * 60f; // Convert to degrees per second for NavMeshAgent
        agent.acceleration = 8f; // How fast the agent's speed increases from standing still to max velocity
        agent.stoppingDistance = 0.1f; // How close the agent needs to be to its destination for it to stop moving
        agent.autoBraking = true; // To prevent walking past the desired destination
        agent.updateRotation = true; // Let the agent handle rotation
        agent.updatePosition = true; // Let the agent handle position
    }

    protected virtual void MoveInDirection(Vector3 desiredDirection)
    {
        if (agent == null) return;
        desiredDirection.y = 0;
        currentDirection = Vector3.Slerp(currentDirection, desiredDirection, Time.deltaTime * 5f).normalized;
        Vector3 targetPos = transform.position + currentDirection * _curMoveSpeed * Time.deltaTime;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPos, out hit, 1f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    protected virtual void Wander()
    {
        if (agent == null) return;
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }

            wanderTimer = randomWanderPointInterval.GetRandom();
        }
        else if (Vector3.Distance(agent.destination, transform.position) <= minDistanceToWanderPoint)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection.y = 0;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }
    }

    protected virtual void GoTo(Vector3 targetPosition)
    {
        if (agent == null) return;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}
