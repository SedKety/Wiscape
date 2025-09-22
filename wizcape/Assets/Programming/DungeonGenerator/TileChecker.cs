using UnityEngine;

public class TileChecker : MonoBehaviour
{
    [SerializeField] private bool isEnemyTile;
    [SerializeField] private bool isChestTile;
    public bool CheckIfEnemyTile()
    {
        return isEnemyTile;
    }

    public bool IsChestTile()
    {
        return isChestTile;
    }
}
