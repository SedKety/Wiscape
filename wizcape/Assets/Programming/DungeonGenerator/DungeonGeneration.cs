using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGeneration : MonoBehaviour
{
    public static DungeonGeneration Instance;
    [SerializeField] private List<SpawnableRoom> rooms = new List<SpawnableRoom>();
    [SerializeField] private List<GameObject> spawnedRooms = new List<GameObject>(); // List of spawned rooms
    [SerializeField] private int roomAmount; // Amount of rooms that will be spawned

    private RoomType _lastRoomType;

    public void Start() { InstantiateRooms(); }

    private void InstantiateRooms()
    {
        var doorPos = transform;

        var startRoom = rooms.Where(x => x.roomType == RoomType.Start).FirstOrDefault();
        if (startRoom) { doorPos = startRoom.InstantiateRoom(doorPos); }
        else { print("No start room found!"); return; }

        for (int i = 0; i < roomAmount; i++)
        {

            var RR = RandomRoom();
            _lastRoomType = RR.roomType;
            doorPos = Instantiate(RR.roomGO,
                doorPos.position,
                Quaternion.identity).
                GetComponent<RoomInitialiser>().
                GetDoorFramePos();
        }
        var randomRoom = rooms.Where(x => x.roomType == RoomType.BossRoom).FirstOrDefault();
        _lastRoomType = randomRoom.roomType;
        var room = Instantiate(randomRoom.roomGO,
            doorPos.position,
            Quaternion.identity);

    }

    private SpawnableRoom RandomRoom()
    {
        //We want interchanging rooms, so never enemy -> enemy. 
        return rooms.Where(x => x.roomType != RoomType.Start &&
        x.roomType != RoomType.End &&
        x.roomType != RoomType.BossRoom &&
        x.roomType != _lastRoomType).
        ToList().
        RandomItem();
    }
}