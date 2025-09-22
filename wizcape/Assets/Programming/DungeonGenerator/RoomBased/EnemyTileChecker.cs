using UnityEngine;

public class EnemyTileChecker : MonoBehaviour
{
    [SerializeField] private bool isEnemyTile;
    public bool CheckIfEnemyTile()
    {
        return isEnemyTile;
    }
}
