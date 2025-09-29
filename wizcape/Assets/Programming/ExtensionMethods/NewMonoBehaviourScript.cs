using System.Collections.Generic;
using UnityEngine;

public static class NewMonoBehaviourScript 
{
    public static T RandomItem<T>(this T[] A) => A[Random.Range(0, A.Length)];
    public static T RandomItem<T>(this List<T> A) => A[Random.Range(0, A.Count)];
}
