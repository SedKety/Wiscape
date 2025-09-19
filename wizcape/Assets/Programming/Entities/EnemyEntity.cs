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
    [SerializeField] protected float moveSpeed; // How fast the enemy moves
    [SerializeField] protected float runningSpeed; // How fast the enemy moves when running
    protected float _curMoveSpeed; // The current movespeed set by the method that invokes GoTo
    protected Vector3 currentDirection; // Current direction the enemy is moving in

    [Header("NavMesh settings")]
    [SerializeField] protected float wanderRadius; // From how far away wander points can be chosen
    [SerializeField] protected float randomWanderPointInterval; // Time between wander destination changes
    [SerializeField] protected float rotationSpeed; // How fast the enemy rotates to face the movement direction
    protected NavMeshAgent agent;
    private float wanderTimerCounter; // For how long the enemy has been wandering toward the current destination

    protected void Awake()
    {
        _curMoveSpeed = moveSpeed;
        currentDirection = transform.forward;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = _curMoveSpeed;
        agent.angularSpeed = rotationSpeed * 60f; // Convert to degrees per second for NavMeshAgent
        agent.acceleration = 8f;
        agent.stoppingDistance = 0.1f;
        agent.autoBraking = true;
        agent.updateRotation = true;
        agent.updatePosition = true;

        wanderTimerCounter = randomWanderPointInterval;
    }

    protected virtual void MoveInDirection(Vector3 desiredDirection)
    {
        if (agent == null) return;

        desiredDirection.y = 0;
        currentDirection = Vector3.Slerp(currentDirection, desiredDirection, Time.deltaTime * 5f).normalized;

        // Calculate a target position in the desired direction
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

        wanderTimerCounter -= Time.deltaTime;
        if (wanderTimerCounter <= 0f)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
            randomPoint.y = transform.position.y; // 2D movement, prevents the bastard from flying off to space

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            wanderTimerCounter = randomWanderPointInterval;
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