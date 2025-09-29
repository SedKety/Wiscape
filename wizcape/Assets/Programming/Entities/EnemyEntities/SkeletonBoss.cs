using EditorAttributes;
using UnityEngine;

public class SkeletonBoss : EnemyEntity
{
    [GUIColor(GUIColor.White)]
    [Header("Skeleton Boss Specifics")]
    [SerializeField] private GameObject[] bonePrefabs;
    [SerializeField] private RandomIntV2 numberOfBonesPerRow;
    [SerializeField] private RandomIntV2 numberOfRows;
    [SerializeField] private SpawnBox boneSpawnBox;

    // 2D array to hold rows of bones
    private GameObject[][] bones;

    [Header("Orbit Settings")]
    [SerializeField] private RandomFloatV2 orbitRadius; // min/max radius
    [SerializeField] private RandomFloatV2 orbitSpeed; // min/max speed per row

    // Per-row data
    private float[] rowAngleOffsets;   // Current angle offsets
    private float[] rowRotationSpeeds; // Rotation speeds
    private float[] orbitRadiusses; 

    private void Start()
    {
        SpawnBones();
    }

    private void SpawnBones()
    {
        int rowCount = numberOfRows.GetRandom();
        bones = new GameObject[rowCount][];
        rowAngleOffsets = new float[rowCount];
        rowRotationSpeeds = new float[rowCount];
        orbitRadiusses = new float[rowCount];

        for (int i = 0; i < rowCount; i++)
        {
            var _orbitSpeed = orbitSpeed.GetRandom();
            var _orbitRadius = orbitRadius.GetRandom() * (i + 1) / (float)rowCount * 2;

            // Randomize initial rotation and speed for this row
            rowAngleOffsets[i] = Random.Range(0f, Mathf.PI * 4f);
            orbitRadiusses[i] = _orbitRadius;
            rowRotationSpeeds[i] = _orbitSpeed * (i - rowCount);

            // Create bones for this row
            int boneCount = numberOfBonesPerRow.GetRandom();
            bones[i] = new GameObject[boneCount];

            for (int j = 0; j < boneCount; j++)
            {
                bones[i][j] = boneSpawnBox.SpawnItem(bonePrefabs.RandomItem());
            }
        }
    }
    private void Update()
    {
        OrbitBones();
    }

    private void OrbitBones()
    {
        if (bones == null || bones.Length == 0) return;

        for (int i = 0; i < bones.Length; i++)
        {
            int boneCount = bones[i].Length;
            if (boneCount == 0) continue;
            orbitRadiusses[i] += Mathf.Sin(Time.time) * 0.01f;
            for (int j = 0; j < boneCount; j++)
            {
                float angle = rowAngleOffsets[i] + j * Mathf.PI * 2f / boneCount;

                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * orbitRadiusses[i],
                    0, 
                    Mathf.Sin(angle) * orbitRadiusses[i]
                );

                bones[i][j].transform.position = transform.position + offset;
            }

            // Rotate this row
            rowAngleOffsets[i] += rowRotationSpeeds[i] * Mathf.Deg2Rad * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
            Gizmos.color = boneSpawnBox.GizmoColor;
            Gizmos.DrawWireCube(boneSpawnBox.BoxPos + transform.position, boneSpawnBox.BoxSize);
    }
}
