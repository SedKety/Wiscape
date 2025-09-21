using UnityEngine;

public class EnemyRoomStats : MonoBehaviour
{
    [SerializeField] private GameObject key;
    private bool _hasKey;

    public void SetKey()
    {
        _hasKey = true;
    }

    private void OnDestroy()
    {
        if (_hasKey)
        {
            SpawnKey();
        }
    }

    private void SpawnKey()
    {
        Instantiate(key, transform.position, Quaternion.identity);
    }
}
