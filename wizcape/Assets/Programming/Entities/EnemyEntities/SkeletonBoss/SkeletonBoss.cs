using EditorAttributes;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public enum SkeletonBossAttack
{
    Rain,
    Orbit
}

[System.Serializable]
public struct SmallBoneData
{
    public GameObject bone;
    public Vector3 startPos;
    public Vector3 targetPos;
    public GameObject warning;
    public Vector3 landingPos;
}

[System.Serializable]
public struct BoneTargetData
{
    public Vector3 groundPos;
    public Vector3 upperPos;
}

public class SkeletonBoss : EnemyEntity
{
    [GUIColor(GUIColor.White)]
    [Header("Skeleton Boss Specifics")]
    [SerializeField] private SkeletonBossAttack attackType;

    [Header("Bone Settings")]
    [SerializeField] private GameObject[] bonePrefabs;
    [SerializeField] private GameObject[] smallBonePrefabs;
    [SerializeField] private RandomIntV2 numberOfBonesPerRow;
    [SerializeField] private RandomIntV2 numberOfRows;
    [SerializeField] private SpawnBox boneSpawnBox;

    [Header("Skeleton Spawner")]
    [SerializeField] private SkeletonBossSkeletonSpawner skeletonSpawner;
    [SerializeField] private float skeletonSpawnDelay = 2f; // Delay before spawning skeletons in Rain attack
    private float lastSkeletonSpawnTime;
    private bool hasSpawnedSkeletonsInRain;

    // 2D array to hold rows of bones
    private GameObject[][] _bones;

    // Attack components
    private SkeletonBossRainAttack rainAttack;
    private SkeletonBossOrbitAttack orbitAttack;

    protected override void Awake()
    {
        // Initialize bone information
        int rowCount = numberOfRows.GetRandom();
        _bones = new GameObject[rowCount][];

        // Get attack components
        rainAttack = GetComponent<SkeletonBossRainAttack>();
        orbitAttack = GetComponent<SkeletonBossOrbitAttack>();
        skeletonSpawner = GetComponent<SkeletonBossSkeletonSpawner>();

        // Pass shared data to attack components
        if (rainAttack != null)
        {
            rainAttack.Initialize(_bones, bonePrefabs, smallBonePrefabs, boneSpawnBox);
        }
        if (orbitAttack != null)
        {
            orbitAttack.Initialize(_bones, transform);
        }
    }

    private void Start()
    {
        SpawnBones();
        lastSkeletonSpawnTime = Time.time;
        hasSpawnedSkeletonsInRain = false;
    }

    private void Update()
    {
        switch (attackType)
        {
            case SkeletonBossAttack.Rain:
                if (rainAttack != null)
                {
                    rainAttack.ExecuteUpdate();
                    // Check if in Waiting state to spawn skeletons
                    if (rainAttack.GetCurrentRainState() == RainState.Waiting && !hasSpawnedSkeletonsInRain && Time.time - lastSkeletonSpawnTime >= skeletonSpawnDelay)
                    {
                        skeletonSpawner?.SpawnSkeletons();
                        hasSpawnedSkeletonsInRain = true;
                    }
                    if (rainAttack.IsAttackComplete())
                    {
                        attackType = SkeletonBossAttack.Orbit;
                        orbitAttack?.Reset();
                        hasSpawnedSkeletonsInRain = false;
                        lastSkeletonSpawnTime = Time.time;
                    }
                }
                break;
            case SkeletonBossAttack.Orbit:
                // Orbit handled in FixedUpdate
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (attackType)
        {
            case SkeletonBossAttack.Rain:
                if (rainAttack != null)
                {
                    rainAttack.ExecuteFixedUpdate();
                }
                break;
            case SkeletonBossAttack.Orbit:
                if (orbitAttack != null)
                {
                    orbitAttack.ExecuteFixedUpdate();
                }
                break;
        }
    }

    public void CleanupAllSkeletons()
    {
        skeletonSpawner?.CleanupSkeletons();
        hasSpawnedSkeletonsInRain = false;
        lastSkeletonSpawnTime = Time.time;
    }

    private void SpawnBones()
    {
        if (bonePrefabs == null || bonePrefabs.Length == 0) return;

        int rowCount = _bones.Length;

        for (int i = 0; i < rowCount; i++)
        {
            int boneCount = numberOfBonesPerRow.GetRandom();
            _bones[i] = new GameObject[boneCount];

            for (int j = 0; j < boneCount; j++)
            {
                _bones[i][j] = boneSpawnBox.SpawnItem(bonePrefabs.RandomItem());
                _bones[i][j].transform.parent = transform;

                // Ensure Rigidbody on big bone
                Rigidbody rb = _bones[i][j].GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = _bones[i][j].AddComponent<Rigidbody>();
                }
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

        // Update attack components with spawned bones
        if (rainAttack != null)
        {
            rainAttack.SetBones(_bones);
        }
        if (orbitAttack != null)
        {
            orbitAttack.SetBones(_bones);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = boneSpawnBox.GizmoColor;
        Gizmos.DrawWireCube(boneSpawnBox.BoxPos + transform.position, boneSpawnBox.BoxSize);
    }
}