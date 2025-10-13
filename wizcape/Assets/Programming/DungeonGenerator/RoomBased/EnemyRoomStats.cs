using UnityEngine;

public class EnemyRoomStats : MonoBehaviour
{
    [SerializeField] private GameObject key;
    [SerializeField] private float yPositionAdder;
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
            Vector3 position = transform.position + new Vector3(0, yPositionAdder, 0);
            Instantiate(key, position, Quaternion.identity);

        }
    }
}
