using UnityEngine;

public class EnemyRoomStats : MonoBehaviour
{
    private bool _hasKey;

    public void SetKey()
    {
        _hasKey = true;
    }
    public bool CheckIfKey()
    {
        return _hasKey;
    }
}
