using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public enum RoomKind
{
    Nothing,
    ChestKey,
    EnemyKey
}

public class RoomInitialiser : MonoBehaviour
{
    [SerializeField] private LootTable lootTable;
    [SerializeField] private RoomKind roomKind;

    [Header("Enemy Spawning")]
    [SerializeField] private bool spawnEnemies;
    [SerializeField] private List<GameObject> possibleEnemies = new List<GameObject>();
    [SerializeField] private RandomIntV2 enemyAmount;
    [SerializeField] private SpawnBox[] enemySpawnBoxes;
    [SerializeField] private SpawnBox[] chestSpawnBoxes;
    [SerializeField] private LayerMask groundLayer;

    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    [Header("Item Handling")]
    [SerializeField] private GameObject chest;
    

    [Header("Key Handling")]
    [SerializeField] private GameObject key;
    [SerializeField] private bool shouldSpawnKey;

    [Header("Door randomisation variables")]
    [SerializeField] private bool randomizedDoor;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject[] possibleDoorWalls;

    private Transform _doorFrame;
    
    public Transform GetDoorFramePos() => randomizedDoor == true ? SpawnRandomDoor() : _doorFrame = door.transform;
    private void HandleKeyPlacement() => _spawnedEnemies.RandomItem().GetComponent<EnemyRoomStats>().SetKey();

    private void Start() 
    { 
        if(spawnEnemies) 
        {
            SpawnEnemies(); 
        } 

        if (shouldSpawnKey)
        {
            HandleKeyPlacement();
        }

        if (chestSpawnBoxes.Length <= 0) { return; }
        SpawnChest();
    }
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyAmount.GetRandom(); i++)
        {
            GameObject enemy = enemySpawnBoxes.RandomItem()
                .SpawnItem(possibleEnemies.RandomItem(),transform.position, groundLayer);

            _spawnedEnemies.Add(enemy.gameObject);
        }
    }

    public Transform SpawnRandomDoor()
    {
        var wall = possibleDoorWalls.RandomItem();

        var spawnedDoor = Instantiate(door, wall.transform.position, Quaternion.identity).transform;

        Destroy(wall);
        return spawnedDoor;
    }

    private void SpawnChest()
    {
        GameObject chestClone = chestSpawnBoxes.RandomItem().SpawnItem(chest, transform.position, groundLayer);
        if(chestClone == null) { print("The fuck?"); }
        var cb = chestClone.GetComponent<ChestBehaviour>();
            cb.PutItemInChest(lootTable
            .GetRandomItem());
    }

    private void OnDrawGizmos()
    {
        foreach (var box in enemySpawnBoxes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(box.BoxPos + transform.position, box.BoxSize);
        }
        foreach(var box in chestSpawnBoxes)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(box.BoxPos + transform.position, box.BoxSize);
        }
    }
}

