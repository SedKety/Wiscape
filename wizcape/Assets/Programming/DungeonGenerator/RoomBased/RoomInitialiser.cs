using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomInitialiser : MonoBehaviour
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


    [Header("Key Handling")]
    [SerializeField] private GameObject key;



    [Header("Door randomisation variables")]
    [SerializeField] private bool randomizedDoor;
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject[] possibleDoorWalls;

    private Transform _doorFrame;

    public Transform GetDoorFramePos() => randomizedDoor == true ? SpawnRandomDoor() : _doorFrame = door.transform;

    public Transform SpawnRandomDoor()
    {
        var wall = possibleDoorWalls.RandomItem();
        var spawnedDoor = Instantiate(door, wall.transform.position, Quaternion.identity).transform;
        Destroy(wall);
        return spawnedDoor;
    }

    public void GenerateRoom()
    {
        HandleEnemyPlacements();

        ChooseRandomState();
    }

    private void ChooseRandomState()
    {

        roomKind = (RoomKind)UnityEngine.Random.Range(0, Enum.GetNames(typeof(RoomKind)).Length);

        switch (roomKind)
        {
            case RoomKind.Nothing:
                //Nothing happens here.
                break;
            case RoomKind.ChestKey:
                //Gotta write the stuff here.
                break;
            case RoomKind.EnemyKey:
                HandleKeyPlacement();
                //DoorPlacement();
                break;
        }



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

    private void HandleKeyPlacement() => _spawnedEnemies.RandomItem().GetComponent<EnemyRoomStats>().SetKey();



}
