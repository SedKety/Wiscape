using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{

    public static DungeonGeneration Instance;
    [SerializeField] private List<GameObject> rooms = new List<GameObject>();
    [SerializeField] private List<Transform> spawnedRooms = new List<Transform>();
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private int roomAmount;
    [SerializeField] private float roomOffset;

    private List<Transform> _allWalls = new List<Transform>();
    private GameObject _disabledWall;
    private GameObject _spawnedDoor;
    private bool _isCollidingWithAnotherRoom;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Start()
    {
        StartCoroutine(GenerateRooms());
    }

    private IEnumerator GenerateRooms()
    {
        for (int i = 0; i < roomAmount; i++)
        {
            if (_isCollidingWithAnotherRoom)
            {
                HandleRoomLogicAgain();
                i--;
            }
            SpawnRandomRoom(i);
            yield return null;
            yield return null;
            yield return null;
        }


        for (int i = 0; i < spawnedRooms.Count; i++)
        {
            spawnedRooms[i].GetComponent<RoomGenerator>().GenerateRoom();
        }
    }

    private void HandleRoomLogicAgain()
    {
        _disabledWall.SetActive(true);
        Destroy(_spawnedDoor);
        int roomIndex = spawnedRooms.Count - 1;

        Destroy(spawnedRooms[roomIndex].gameObject);

        spawnedRooms.RemoveAt(roomIndex);

    }

    private void SpawnRandomRoom(int i)
    {

        Transform chosenRoom = Instantiate(rooms.RandomThing()).transform;

        if (spawnedRooms.Count == 0)
        {
            HandleFirstRoom(chosenRoom);
        }

        else
        {
            HandleOtherRoom(i, chosenRoom);
        }
    }

    private void HandleFirstRoom(Transform room)
    {
        room.position = Vector3.zero;
        room.rotation = Quaternion.identity;
        spawnedRooms.Add(room);
    }

    private void HandleOtherRoom(int i, Transform room)
    {
        Transform previousRoom = spawnedRooms[i - 1];

        if (!_isCollidingWithAnotherRoom)
        {
            _allWalls = MakeWallsList(previousRoom.GetChild(0));

        }
        Transform previousRoomWall = ChooseRandomWall(_allWalls);

        _disabledWall = previousRoomWall.gameObject;
        _spawnedDoor = Instantiate(doorPrefab, previousRoomWall.position, previousRoomWall.rotation, previousRoom);

        _disabledWall.SetActive(false);

        Transform randomNewWall = ChooseRandomWallFromNewRoom(room);

        randomNewWall.parent = null;
        room.parent = randomNewWall;
        Vector3 roomSpawnRotation = FindRoomSpawnRotation(_spawnedDoor.transform);
        Vector3 roomSpawnPosition = FindRoomSpawnPosition(_spawnedDoor.transform);

        randomNewWall.eulerAngles = roomSpawnRotation;
        randomNewWall.position = roomSpawnPosition;

        room.parent = null;
        Destroy(randomNewWall.gameObject);
        spawnedRooms.Add(room);

        _isCollidingWithAnotherRoom = false;
    }

    private List<Transform> MakeWallsList(Transform wallFolder)
    {
        List<Transform> wallList = new List<Transform>();

        for (int i = 0; i < wallFolder.childCount; i++)
        {
            wallList.Add(wallFolder.GetChild(i));
        }

        return wallList;
    } 
    private Transform ChooseRandomWall(List<Transform> walls)
    {
        int randomWallIndex = Random.Range(0, walls.Count);

        return walls[randomWallIndex];
    }

    private Transform ChooseRandomWallFromNewRoom(Transform room)
    {

        Transform wallFolder = room.GetChild(0);
        int randomWallIndex = Random.Range(0, wallFolder.childCount);

        return wallFolder.GetChild(randomWallIndex);
    }

    private Vector3 FindRoomSpawnPosition(Transform doorTransform)
    {
        Vector3 spawnPosition = doorTransform.position + doorTransform.right * roomOffset;


        return spawnPosition;
    }

    private Vector3 FindRoomSpawnRotation(Transform doorTransform)
    {
        return doorTransform.eulerAngles + new Vector3(0, 180, 0);
    }

    public void CollisionCheck()
    {
        _isCollidingWithAnotherRoom = true;
    }
}
