using System;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Transform spawnedDoor;

    [Header("Enemy Spawning")]
    [SerializeField] private bool spawnEnemies;
    [SerializeField] private List<GameObject> possibleEnemies = new List<GameObject>();
    [SerializeField] private RandomIntV2 enemyAmount;
    [SerializeField] private SpawnBox[] enemySpawnBoxes;
    [SerializeField] private SpawnBox[] chestSpawnBoxes;
    [SerializeField] private List<SpawnBox> trapSpawnBoxes;
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

    [Header("Locked room randomisation variables")]
    [SerializeField] private int lockedRoomChance;

    [Header("Trap Variables")]
    [SerializeField] private int minTrapAmount;
    [SerializeField] private int maxTrapAmount;
    [SerializeField] private GameObject[] possibleTraps;


    private Transform _doorFrame;
    
    public Transform GetDoorFramePos() => randomizedDoor == true ? SpawnRandomDoor() : _doorFrame = door.transform;
    private void HandleKeyPlacement()
    {
        if (_spawnedEnemies.Count > 0)
        {
            _spawnedEnemies.RandomItem().GetComponent<EnemyRoomStats>().SetKey();
        }

        else
        {
            SpawnChest(key);
        }
    } 

    private void Start() 
    { 
        if(spawnEnemies) 
        {
            SpawnEnemies(); 
        }

        if (shouldSpawnKey)
        {
            HandleLockedRoom();
        }

        SpawnTraps();
        if (chestSpawnBoxes.Length <= 0) { return; }
        SpawnChest(null);
    }
    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyAmount.GetRandom(); i++)
        {
            GameObject enemy = enemySpawnBoxes.RandomItem()
                .SpawnItem(possibleEnemies.RandomItem(),transform.position, groundLayer);
            _spawnedEnemies.Add(enemy);
        }
    }

    public Transform SpawnRandomDoor()
    {
        var wall = possibleDoorWalls.RandomItem();

        spawnedDoor = Instantiate(door, wall.transform.position, Quaternion.identity).transform;

        
        
        Destroy(wall);
        return spawnedDoor;
    }

    private void SpawnChest(GameObject obj)
    {

        if (chestSpawnBoxes.Length == 0) return;
        GameObject chestClone = chestSpawnBoxes.RandomItem().SpawnItem(chest, transform.position, groundLayer);

       
        var cb = chestClone.GetComponent<ChestBehaviour>();

        if (obj == null)
        {
            cb.PutItemInChest(lootTable.GetRandomItem());
        }

        else
        {
            cb.PutItemInChest(obj);
        }
    }

    private void HandleLockedRoom()
    {
        int lockedRoomNumber = UnityEngine.Random.Range(0, lockedRoomChance);

        if (lockedRoomNumber == 0) 
        {
            HandleKeyPlacement();
            LockRoom();
        }
    }

    private void LockRoom()
    {
        door.GetComponentInChildren<LockBehaviour>().LockDoor();
        print("LOCKED");
    }

    private void SpawnTraps()
    {

        int trapAmount = UnityEngine.Random.Range(minTrapAmount, maxTrapAmount + 1);
        if (trapSpawnBoxes.Count == 0) return;

        for (int i = 0; i < trapAmount; i++)
        {
            SpawnBox spawnBox = trapSpawnBoxes.RandomItem();
            spawnBox.SpawnItem(possibleTraps.RandomItem(), transform.position, groundLayer);

            if (spawnBox.hasToBeRemoved)
            {
                trapSpawnBoxes.Remove(spawnBox);
            }
        }

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

        foreach (var box in trapSpawnBoxes)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(box.BoxPos + transform.position, box.BoxSize);
        }
    }
}
