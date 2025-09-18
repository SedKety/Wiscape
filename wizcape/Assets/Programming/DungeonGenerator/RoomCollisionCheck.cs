using UnityEngine;

public class RoomCollisionCheck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RoomCollider"))
        {
            DungeonGeneration.Instance.CollisionCheck();
        }
    }
}
