using UnityEngine;

[System.Serializable]
[Tooltip("A struct that returns a random float between a set min and max value, usefull for randomized timings")]
public struct RandomIntV2
{
    [Tooltip("Minimum value that can be returned")]
    public int min;

    [Tooltip("Maximum value that can be returned")]
    public int max;

    [Tooltip("The last value that was returned")]
    public int Last { get { return last; } private set { last = value; } }

    private int last;

    [Tooltip("Returns a random float between the set min and max values")]
    public int GetRandom()
    {
        return Last = Random.Range(min, max);
    }
}
