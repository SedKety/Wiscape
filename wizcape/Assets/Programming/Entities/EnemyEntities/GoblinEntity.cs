using System.Collections;
using UnityEngine;

public class GoblinEntity : EnemyEntity
{
    private float _defaultAngularSpeed; // Store default rotation speed to restore after retreat

    protected override void Awake()
    {
        base.Awake();
        _defaultAngularSpeed = _agent.angularSpeed; 
    } 

    protected override void Update()
    {
        base.Update();
        if (moveAction != MoveActions.Retreat && _agent != null && _agent.angularSpeed != _defaultAngularSpeed)
        {
            _agent.angularSpeed = _defaultAngularSpeed;
            Debug.Log($"[{Time.time}] Restored default angular speed: {_agent.angularSpeed}");
        }
    }

    public override void TakeDamage(int intakeDamage, DamageLayer dl, DamageType dt = DamageType.physical)
    {
       health -= intakeDamage;
        SoundTriggerScript.Instance.SetSound(painSound);
        if(health <= 0)
        {
            print($"Entity {gameObject.name}, has died");
            StartCoroutine(PlayAnimation());
        }
        if (_agent != null)
        {
            _agent.ResetPath(); // Clear current path to stop ongoing movement
            _agent.angularSpeed = _defaultAngularSpeed * 2f; 
        }

        _timeSpentRetreating = 0f; 
        _curRetreatTime = retreatTime.GetRandom(); 
        moveAction = MoveActions.Retreat; 

        Retreat();

        Debug.Log($"[{Time.time}] Goblin took damage, retreating immediately. Destination: {_agent.destination}");
    }

    private IEnumerator PlayAnimation()
    {
        _animator.SetTrigger("Die");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
    protected override MoveActions CalculateMoveAction()
    {
        MoveActions ma = distanceToPlayer switch
        {
            var d when d == Distance.WithinRange => CanStrikePlayer() ? MoveActions.Strike : MoveActions.Advance, // Stay in place if can't strike
            var d when d == Distance.Close =>  MoveActions.Advance,
            var d when d == Distance.Medium => MoveActions.Advance,
            var d when d == Distance.Far => MoveActions.Advance, // Replace Run with Advance
            var d when d == Distance.OutOfRange => MoveActions.Wander,
            _ => MoveActions.Unknown
        };


        if (moveAction == MoveActions.Retreat && _timeSpentRetreating < _curRetreatTime)
        {
            ma = MoveActions.Retreat;
        }

        return ma;
    }

    protected override void Retreat()
    {
        _curMoveSpeed = runningSpeed.Last;
        Vector3 directionAwayFromPlayer = (transform.position - playerGO.transform.position).normalized;
        float randomAngle = Random.Range(-90f, 90f);
        Vector3 retreatDir = Quaternion.Euler(0, randomAngle, 0) * directionAwayFromPlayer;
        Vector3 retreatPos = transform.position + retreatDir * retreatDistance;
        retreatPos.y = transform.position.y;

        GoTo(retreatPos);
    }
}