using UnityEngine;

public class Ticker : MonoBehaviour
{
    [SerializeField] private float timePerTick;

    [SerializeField] private float timeTillNextTick;

    public delegate void TickAction();
    public static event TickAction OnTickAction;

    void Update()
    {
        timeTillNextTick += Time.deltaTime;
        if (timeTillNextTick >= timePerTick)
        {
            timeTillNextTick = 0;
            TickEvent();
        }
    }

    void TickEvent()
    {
        OnTickAction?.Invoke();
    }
}
