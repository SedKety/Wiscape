using System.Collections;
using UnityEngine;

public class SkeletonEntity : EnemyEntity
{



    [Header("Movement variables")]
    private MoveActions currentMoveAction = MoveActions.Idle; //The movement action the enemy will perform
    private float chaseTime; // Time the enemy will chase the player for
    private float timeSpentIdle; // Time spent idling
    private float pDistance; // Distance to player

    [Header("Combat variables")]
    [Tooltip("Time till the next attack action can be performed")]
    [SerializeField] private float attackDelay = 0; 
    [SerializeField] private float timeSinceLastHit = 0; // Time since the enemy last attacked something



    [Header("Fuzzified variables")]
    [SerializeField] private Distance distanceToPlayer = Distance.OutOfRange; //Fuziffied version of pDistance
    [SerializeField] private MoveActions moveAction = MoveActions.Wander; //Fuzzified result of distance and other variables


    protected void Start()
    {
        playerGO = PlayerLocation.GetPlayer();
        timeSinceLastHit = attackDelay; // So the enemy can attack immediately
    }
    protected override void Update()
    {
        timeSinceLastHit += Time.deltaTime;
        Fuzzify();
        Defuzzify();
    }


    private void Defuzzify()
    {
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
                // Move away from the player
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

    private Distance CalculateDistance()
    {
        pDistance = Vector3.Distance(transform.position, playerGO.transform.position);

        //c#8 is ass, c#9 would've made this so much easier
        Distance d = pDistance switch
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
        if (attackDelay >= timeSinceLastHit) { return false; }
        var hitRays = FireRays();
        for (int i = 0; i < hitRays.Length; i++)
        {
            if (!hitRays[i].collider) { continue; }
            if (hitRays[i].collider.gameObject == playerGO)
            {
                return true;
            }
        }
        return false;
    }
    private MoveActions CalculateMoveAction()
    {
        //Same goes for this, c#9 is way better....
        MoveActions ma = distanceToPlayer switch
        {
            var d when d == Distance.WithinRange =>
                CanStrikePlayer() ? MoveActions.Strike : MoveActions.Retreat,

            var d when d == Distance.Close =>
                timeSinceLastHit >= attackDelay ? MoveActions.Advance : MoveActions.Await,

            var d when d == Distance.Medium =>
                MoveActions.Advance,

            var d when d == Distance.Far =>
                MoveActions.Run,

            var d when d == Distance.OutOfRange =>
                MoveActions.Wander,

            _ => MoveActions.Unknown
        };

        return ma;
    }
    #endregion

    #region Defuzzified actions

    public void Run()
    {
        _curMoveSpeed = runningSpeed;
        GoTo(playerGO.transform.position);
    }
    public void Advance()
    {
        _curMoveSpeed = moveSpeed;
        GoTo(playerGO.transform.position);
    }
    public void Idle()
    {

    }

    private void Strike(GameObject target)
    {
        timeSinceLastHit = 0; // Reset the time since last hit

        damageInstance.Execute(target); //Attacks the target
    }
    #endregion

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (!playerGO) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rayOrigin.position, (playerGO.transform.position - rayOrigin.position).normalized * pDistance);
    }
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
