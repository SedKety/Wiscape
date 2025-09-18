using UnityEngine;

public class WeaponFloating : MonoBehaviour
{
    public float frequency;
    public float amplitude;

    private bool _grabbed;
    private void Update()
    {
        if (!_grabbed)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Sin(Time.time * frequency) * amplitude, transform.localPosition.z);
        }
    }

    public void GetGrabbed()
    {
        _grabbed = true;
    }
}
