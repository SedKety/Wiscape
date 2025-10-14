using UnityEngine;

public class RandomRot : MonoBehaviour
{
    public bool x, y, z;
    private void Awake()
    {
        transform.rotation = Quaternion.Euler(
            x ? Random.Range(0, 360) : transform.rotation.eulerAngles.x,
            y ? Random.Range(0, 360) : transform.rotation.eulerAngles.y,
            z ? Random.Range(0, 360) : transform.rotation.eulerAngles.z
            );
    }
}
