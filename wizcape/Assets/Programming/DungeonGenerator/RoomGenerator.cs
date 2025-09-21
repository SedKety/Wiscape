using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    [Header("Enemy Handling")]

    [SerializeField] private List<GameObject> possibleEnemies = new List<GameObject>();
    [SerializeField] private List<Transform> enemySpawnTiles = new List<Transform>();
    [SerializeField] private int minEnemyAmount, maxEnemyAmount;

    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    [Header("Item Handling")]
    [SerializeField] private GameObject chest;


    [Header("Key Handling")]
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    private Transform _doorFrame;
   
    public void GenerateRoom()
    {
        HandleEnemyPlacements();
        HandleKeyPlacement();
        DoorPlacement();
    }

    private void HandleEnemyPlacements()
    {

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).GetComponent<EnemyTileChecker>().CheckIfEnemyTile())
            {
                enemySpawnTiles.Add(transform.GetChild(1).GetChild(i));
            }
        }
        int enemyAmount = Random.Range(minEnemyAmount, maxEnemyAmount + 1);

        for (int i = 0; i < enemyAmount; i++)
        {
            int randomEnemy = Random.Range(0, possibleEnemies.Count);
            int randomTileIndex = Random.Range(0, enemySpawnTiles.Count);
            Transform randomTile = enemySpawnTiles[randomTileIndex];

            GameObject enemy = Instantiate(possibleEnemies[randomEnemy], new Vector3(randomTile.position.x, randomTile.position.y, randomTile.position.z), Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void HandleKeyPlacement()
    {
        int randomKeyEnemy = Random.Range(0, _spawnedEnemies.Count);

        _spawnedEnemies[randomKeyEnemy].GetComponent<EnemyRoomStats>().SetKey();
    }

    private void DoorPlacement()
    {

        if (transform.childCount <= 3) return;
        _doorFrame = transform.GetChild(3);
        GameObject doorClone = Instantiate(door, _doorFrame.position, _doorFrame.rotation, transform);



    }

    
}
