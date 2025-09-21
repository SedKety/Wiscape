using UnityEngine;

public class EnemyRoomStats : MonoBehaviour
{
    [SerializeField] private GameObject key;
    private bool _hasKey;

    private bool _isQuitting;
    public void SetKey()
    {
        _hasKey = true;
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }
    private void OnDisable()
    {
        if (_hasKey)
        {
            SpawnKey();
        }
    }

    private void SpawnKey()
    {
        if (!_isQuitting)
        {
            Instantiate(key, transform.position, Quaternion.identity);

        }
    }
}
