using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnableRoom", menuName = "ScriptableObjects/Generation/SpawnableRoom")]
public class SpawnableRoom : ScriptableObject
{
    public GameObject roomGO;
    public RoomType roomType;

    public Transform InstantiateRoom(Transform spawnpos)
    {
        var room = Instantiate(roomGO, spawnpos.position, spawnpos.rotation);
        if (room.TryGetComponent(out RoomInitialiser RI))
        {
            return RI.GetDoorFramePos();
        }
        else
        {
            Console.WriteLine("The room does not have a RoomInitialiser component.");
            Destroy(room);
            return null;
        }
    }
}
