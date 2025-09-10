using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{

    public static MonoBehaviour coroutineHost;

    public void Awake()
    {
        coroutineHost = this;
    }
}
