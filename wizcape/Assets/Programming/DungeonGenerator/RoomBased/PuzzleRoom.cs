using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;


public class PuzzleRoom : MonoBehaviour
{
    public const int NumPillars = 3;
    [SerializeField] private LayerMask gemLayer;
    [SerializeField] private Transform[] pillarPositions = new Transform[NumPillars];
    [SerializeField] private PillarAndGem[] pillarsAndGems = new PillarAndGem[NumPillars];

    [SerializeField] private List<Transform> gemSpawnLocations = new List<Transform>();

    private int[] _chosenIndices = new int[NumPillars];

    private void Awake()
    {
        for(int i = 0; i < NumPillars; i++)
        {
            _chosenIndices[i] = UnityEngine.Random.Range(0, NumPillars);
            Instantiate(pillarsAndGems[_chosenIndices[i]].pillar, pillarPositions[i].position, pillarPositions[i].rotation, transform);
            var randomSpawnPos = gemSpawnLocations.RandomItem();
            Instantiate(pillarsAndGems[_chosenIndices[i]].gem, randomSpawnPos.position, gemSpawnLocations[i].rotation, transform);
            gemSpawnLocations.Remove(randomSpawnPos);
        }
    }


    [Serializable]
    private struct PillarAndGem
    {
        public GameObject pillar;
        public GameObject gem;
    }
}
