using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    public enum RoomKind
    {
        Nothing,
        ChestKey,
        EnemyKey
    }

    public RoomKind roomKind;
    [Header("Enemy Handling")]

    [SerializeField] private List<GameObject> possibleEnemies = new List<GameObject>();
    [SerializeField] private List<Transform> enemySpawnTiles = new List<Transform>();
    [SerializeField] private int minEnemyAmount, maxEnemyAmount;

    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    [Header("Item Handling")]
    [SerializeField] private GameObject chest;
    [SerializeField] private List<Transform> chestSpawnTiles = new List<Transform>();
    


    [Header("Key Handling")]
    [SerializeField] private GameObject key;
    [SerializeField] private GameObject door;
    private Transform _doorFrame;
   
    public void GenerateRoom()
    {
        HandleEnemyPlacements();

        ChooseRandomState();
    }

    private void ChooseRandomState()
    {

        //roomKind = (RoomKind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(RoomKind)).Length);

        switch(roomKind)
        {
            case RoomKind.Nothing:
                //Nothing happens here.
                break;
            case RoomKind.ChestKey:
                HandleKeyChestPlacement();
                DoorPlacement();
                break;
            case RoomKind.EnemyKey:
                HandleKeyEnemyPlacement();
                DoorPlacement();
                break;
        }


        
    }

    private void HandleKeyChestPlacement()
    {
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).GetComponent<TileChecker>().IsChestTile())
            {
                chestSpawnTiles.Add(transform.GetChild(1).GetChild(i));
            }
        }
        int randomChestTileIndex = UnityEngine.Random.Range(0, chestSpawnTiles.Count);

        Transform randomTile = chestSpawnTiles[randomChestTileIndex];

        Transform randomChest = Instantiate(chest, randomTile.position, Quaternion.identity).transform;

        randomChest.eulerAngles = CalculateRotation(randomTile);

        

    }
    private void HandleEnemyPlacements()
    {

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            if (transform.GetChild(1).GetChild(i).GetComponent<TileChecker>().CheckIfEnemyTile())
            {
                enemySpawnTiles.Add(transform.GetChild(1).GetChild(i));
            }
        }
        int enemyAmount = UnityEngine.Random.Range(minEnemyAmount, maxEnemyAmount + 1);

        for (int i = 0; i < enemyAmount; i++)
        {
            int randomEnemy = UnityEngine.Random.Range(0, possibleEnemies.Count);
            int randomTileIndex = UnityEngine.Random.Range(0, enemySpawnTiles.Count);
            Transform randomTile = enemySpawnTiles[randomTileIndex];

            GameObject enemy = Instantiate(possibleEnemies[randomEnemy], new Vector3(randomTile.position.x, randomTile.position.y, randomTile.position.z), Quaternion.identity);
            _spawnedEnemies.Add(enemy);
        }
    }

    private void HandleKeyEnemyPlacement()
    {
        int randomKeyEnemy = UnityEngine.Random.Range(0, _spawnedEnemies.Count);

        _spawnedEnemies[randomKeyEnemy].GetComponent<EnemyRoomStats>().SetKey();
    }

    private void DoorPlacement()
    {

        if (transform.childCount <= 3) return;
        _doorFrame = transform.GetChild(3);
        GameObject doorClone = Instantiate(door, _doorFrame.position, _doorFrame.rotation, transform);



    }

    private Vector3 CalculateRotation(Transform tile)
    {
        Vector3 direction = tile.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        float yRotation = lookRotation.eulerAngles.y;

        float ySnapped = Mathf.Ceil(yRotation / 90f) * 90f;

        Vector3 rotation = new Vector3(0, ySnapped + 180f, 0);
        return rotation;
    }

    
}
