using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Enemy Handling")]

    [SerializeField] private List<GameObject> possibleEnemies = new List<GameObject>();
    [SerializeField] private List<Transform> spawnTiles = new List<Transform>();
    [SerializeField] private int minEnemyAmount, maxEnemyAmount;
   
    public void GenerateRoom()
    {
        HandleEnemyPlacements();
    }

    private void HandleEnemyPlacements()
    {

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).GetComponent<EnemyTileChecker>().CheckIfEnemyTile())
            {
                spawnTiles.Add(transform.GetChild(1).GetChild(i));
            }
        }
        int enemyAmount = Random.Range(minEnemyAmount, maxEnemyAmount + 1);

        for (int i = 0; i < enemyAmount; i++)
        {
            int randomEnemy = Random.Range(0, possibleEnemies.Count);
            int randomTileIndex = Random.Range(0, spawnTiles.Count);
            Transform randomTile = spawnTiles[randomTileIndex];

            Instantiate(possibleEnemies[randomEnemy], new Vector3(randomTile.position.x, randomTile.position.y + 5, randomTile.position.z), Quaternion.identity);
        }
    }

    
}
