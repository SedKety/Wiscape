using UnityEngine;

public class SlimeEntity : EnemyEntity
{
    protected override void Update()
    {
        base.Update();
    }

    protected override void Retreat()
    {
        Advance();
    }

    protected override MoveActions CalculateMoveAction()
    {
        var MA = base.CalculateMoveAction();
        if (MA == MoveActions.Retreat) MA = MoveActions.Advance;
        if(MA == MoveActions.Run) MA = MoveActions.Advance;
        return MA;
    }
}
