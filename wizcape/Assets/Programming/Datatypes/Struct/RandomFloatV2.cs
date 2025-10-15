using UnityEngine;

[System.Serializable]
[Tooltip("A struct that returns a random float between a set min and max value, usefull for randomized timings")]
public struct RandomFloatV2
{
    [Tooltip("Minimum value that can be returned")]
    public float min;

    [Tooltip("Maximum value that can be returned")]
    public float max;

    [Tooltip("The last value that was returned")]
    public float Last { get { return last; } private set { last = value; } }

    private float last;

    public float PercentageOfMax => (Last - min) / (max - min) * 100;

    [Tooltip("Returns a random float between the set min and max values")]
    public float GetRandom() => Last = Random.Range(min, max); 
    public RandomFloatV2(float min, float max)
    {
        this.min = min;
        this.max = max;
        last = 0f;
    }
}
