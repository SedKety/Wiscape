using System.Collections;
using Unity.Collections;
using UnityEngine;

public class SkeletonEntity : EnemyEntity
{
    [Header("Movement variables")]
    [SerializeField] private float retreatDistance; // Multiplier used to calculate how far away the enemy retreats
    [SerializeField] private float retreatTime; // Time the enemy will retreat for
    private float _timeSpentRetreating; // Time spent retreating

    [Space]

    [SerializeField] private float chaseTime; // Time the enemy will chase the player for
    private float _timeSpentChasing; // Time spent chasing the player

    [SerializeField] private float idleTime; // Time the skeleton will idle for

    private float _pDistance; // Distance to player

    [Header("Combat variables")]
    [Tooltip("Time till the next attack action can be performed")]
    [SerializeField] private float attackDelay = 0;
    private float _timeSinceLastHit = 0; // Time since the enemy last attacked something

    [Header("Fuzzified variables")]
    [SerializeField] private Distance distanceToPlayer = Distance.OutOfRange; //Fuziffied version of _pDistance
    [SerializeField] private MoveActions moveAction = MoveActions.Wander; //Fuzzified result of distance and other variables

    protected void Start()
    {
        playerGO = PlayerLocation.GetPlayer();
        _timeSinceLastHit = attackDelay; // So the enemy can attack immediately
    }


    protected void Update()
    {
        _timeSinceLastHit += Time.deltaTime;
        _timeSpentChasing += Time.deltaTime;
        _timeSpentRetreating += Time.deltaTime;
        Fuzzify();
        Defuzzify();
    }

    private float _timeSpentRetreatingPrev;
    private void Defuzzify()
    {
        _timeSpentRetreatingPrev = _timeSpentRetreating;
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
                // Circle around the player
                break;
            case MoveActions.Retreat:
                _timeSpentRetreating = _timeSpentRetreatingPrev;
                Retreat();
                break;
            case MoveActions.Wander:
                Wander();
                break;
            case MoveActions.StrafeLeft:
                // Strafe left
                break;
            case MoveActions.StrafeRight:
                // Strafe right
                break;
            case MoveActions.Idle:
                // Stand still
                break;
            case MoveActions.Strike:
                Strike(playerGO);
                break;
            default:
                // Default action
                break;
        }
    }

    #region fuzzification
    private void Fuzzify()
    {
        distanceToPlayer = CalculateDistance();
        moveAction = CalculateMoveAction();
    }

    //c#8 is ass, c#9 makes this so much cleaner
    private Distance CalculateDistance()
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

    private bool CanStrikePlayer()
    {
        if (attackDelay >= _timeSinceLastHit) { return false; }
        if (_pDistance <= attackRange) { return true; } // Fixed logic to allow striking when within range
        return false;
    }

    //c#8 is ass, c#9 makes this so much cleaner
    private MoveActions CalculateMoveAction()
    {
        MoveActions ma = distanceToPlayer switch
        {
            var d when d == Distance.WithinRange =>
                CanStrikePlayer() ? MoveActions.Strike : MoveActions.Retreat,

            var d when d == Distance.Close =>
                _timeSinceLastHit >= attackDelay ? MoveActions.Advance : MoveActions.Await,

            var d when d == Distance.Medium =>
                MoveActions.Advance,

            var d when d == Distance.Far =>
                MoveActions.Run,

            var d when d == Distance.OutOfRange =>
                MoveActions.Wander,

            _ => MoveActions.Unknown
        };

        if (_timeSpentRetreating < retreatTime)
        {
            ma = MoveActions.Retreat;
        }

        return ma;
    }
    #endregion

    #region Defuzzified actions
    private void Run()
    {
        _curMoveSpeed = runningSpeed;
        GoTo(playerGO.transform.position);
    }

    private void Advance()
    {
        _curMoveSpeed = moveSpeed;
        GoTo(playerGO.transform.position);
    }

    private void Idle()
    {
        // Stand still
    }

    private void Retreat()
    {
        _curMoveSpeed = runningSpeed;

        Vector3 directionAwayFromPlayer = (transform.position - playerGO.transform.position).normalized;
        Vector3 retreatPos = transform.position + directionAwayFromPlayer * retreatDistance;
        retreatPos.y = transform.position.y; // Flat y for 2D movement

        GoTo(retreatPos);
    }

    private void Strike(GameObject target)
    {
        _timeSinceLastHit = 0; // Reset the time since last hit
        damageInstance.Execute(target); // Attacks the target
    }
    #endregion
}

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