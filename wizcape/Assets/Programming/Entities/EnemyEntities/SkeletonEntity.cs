using System.Collections;
using Unity.Collections;
using UnityEngine;

public class SkeletonEntity : EnemyEntity
{
    [Header("Movement variables")]
    [SerializeField] private float retreatDistance; // Multiplier used to calculate how far away the enemy retreats
    [SerializeField] private RandomFloatV2 retreatTime; // Time the enemy will retreat for
    private float _curRetreatTime;
    private float _timeSpentRetreating; // Time spent retreating

    [Space]

    [SerializeField] private float chaseTime; // Time the enemy will chase the player for
    private float _timeSpentChasing; // Time spent chasing the player
    [SerializeField] private RandomFloatV2 idleTime; // Time the skeleton will idle for
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
        _curRetreatTime = retreatTime.GetRandom();
        playerGO = PlayerLocation.GetPlayer();
        _timeSinceLastHit = attackDelay; // So the enemy can attack immediately
       _timeSpentRetreating = _curRetreatTime; // So the enemy doesn't retreat immediately
    }

    protected void Update()
    {
        agent.speed = _curMoveSpeed;
        _timeSinceLastHit += Time.deltaTime;
        _timeSpentChasing += Time.deltaTime;
        _timeSpentRetreating += Time.deltaTime;

        if(_timeSpentRetreating >= _curRetreatTime) { _curRetreatTime = retreatTime.GetRandom(); }

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
                break;
            case MoveActions.Retreat:
                _timeSpentRetreating = _timeSpentRetreatingPrev;
                Retreat();
                break;
            case MoveActions.Wander:
                Wander();
                break;
            case MoveActions.StrafeLeft:
                break;
            case MoveActions.StrafeRight:
                break;
            case MoveActions.Idle:
                break;
            case MoveActions.Strike:
                Strike(playerGO);
                break;
            default:
                break;
        }
    }

    #region fuzzification
    private void Fuzzify()
    {
        distanceToPlayer = CalculateDistance();
        moveAction = CalculateMoveAction();
    }

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

    private MoveActions CalculateMoveAction()
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

        //Checks whether the skeleton has run far away but not quite out of range,
        //If so, it will start running to the player again
        //Additionally checks if the skeleton is retreating, if so it will continue to retreat until the time is up
        if (distanceToPlayer != Distance.Far && moveAction == MoveActions.Retreat && _timeSpentRetreating < _curRetreatTime)
        {
            ma = MoveActions.Retreat;
        }
        return ma;
    }
    #endregion

    #region Defuzzified actions
    private void Run()
    {
        _curMoveSpeed = runningSpeed.Last;
        GoTo(playerGO.transform.position);
    }

    private void Advance()
    {
        _curMoveSpeed = moveSpeed.Last;
        GoTo(playerGO.transform.position);
    }

    private void Idle()
    {
        // IdleOnline
    }

    private void Retreat()
    {
        _curMoveSpeed = runningSpeed.Last;

        Vector3 directionAwayFromPlayer = (transform.position - playerGO.transform.position).normalized;

        //Fov(179 degrees)
        float randomAngle = Random.Range(-90f, 90f);

        Vector3 retreatDir = Quaternion.Euler(0, randomAngle, 0) * directionAwayFromPlayer;

        Vector3 retreatPos = transform.position + retreatDir * retreatDistance;
        retreatPos.y = transform.position.y;

        GoTo(retreatPos);
    }


    private void Strike(GameObject target)
    {
        _timeSinceLastHit = 0;
        damageInstance.Execute(target);
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

// Signed and aprroved by CANPAI (Goat of python programming)