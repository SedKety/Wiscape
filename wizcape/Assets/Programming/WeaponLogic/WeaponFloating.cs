using UnityEngine;

public class WeaponFloating : MonoBehaviour
{
    public float frequency;
    public float amplitude;

    private bool _grabbed;
    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }
    private void Update()
    {
        if (!_grabbed)
        {
            transform.position = new Vector3(transform.position.x, Mathf.Sin(Time.time * frequency) * amplitude, transform.position.z);
        }
    }

    public void GetGrabbed()
    {
        _grabbed = true;
    }
}
