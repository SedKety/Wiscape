using UnityEngine;

public class PlayerLocation : MonoBehaviour
{
    public static GameObject PlayerGO { get; private set; }
    public static Vector3 GetCurrentPos()
    {
        return PlayerGO.transform.position; 
    }

    public static GameObject GetPlayer()
    {
        return PlayerGO;
    }

    public static Transform GetTransform()
    {
        return PlayerGO.transform;
    }
    private void Awake()
    {
        PlayerGO = gameObject;

    }
}
