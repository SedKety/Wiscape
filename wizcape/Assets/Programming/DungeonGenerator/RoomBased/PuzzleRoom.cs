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

    [SerializeField] private List<Transform> pillars = new List<Transform>();
    [SerializeField] private List<Transform> gemSpawnLocations = new List<Transform>();

    [SerializeField] private Transform door;
    private int[] _chosenIndices = new int[NumPillars];

    private bool _hasCompleted;

    private void Awake()
    {
        door.GetComponent<DoorBehaviour>().SetPuzzleDoor();
        for(int i = 0; i < NumPillars; i++)
        {
            _chosenIndices[i] = UnityEngine.Random.Range(0, NumPillars);
            GameObject pillar = Instantiate(pillarsAndGems[_chosenIndices[i]].pillar, pillarPositions[i].position, pillarPositions[i].rotation, transform);
            pillars.Add(pillar.transform);
            var randomSpawnPos = gemSpawnLocations.RandomItem();
            Instantiate(pillarsAndGems[_chosenIndices[i]].gem, randomSpawnPos.position, gemSpawnLocations[i].rotation, transform);
            gemSpawnLocations.Remove(randomSpawnPos);
        }
    }

    private void Update()
    {
        if (_hasCompleted) return;
        for (int i = 0; i < pillars.Count; i++)
        {
            if (!pillars[i].GetComponent<StoolBehaviour>().IsChecked()) return;
        }

        _hasCompleted = true;
        door.GetComponent<DoorBehaviour>().SolvePuzzle();
    }

    [Serializable]
    private struct PillarAndGem
    {
        public GameObject pillar;
        public GameObject gem;
    }
}
