using UnityEngine;
using System.Collections.Generic;

public class SkeletonBossOrbitAttack : MonoBehaviour
{
    [Header("Orbit Settings")]
    [SerializeField] private RandomFloatV2 orbitRadius;
    [SerializeField] private RandomFloatV2 orbitSpeed;

    private GameObject[][] _bones;
    private Transform bossTransform;
    private float[] rowAngleOffsets;
    private float[] rowRotationSpeeds;
    private float[] orbitRadiusses;

    public void Initialize(GameObject[][] bones, Transform bossTransform)
    {
        _bones = bones;
        this.bossTransform = bossTransform;

        if (_bones != null)
        {
            rowAngleOffsets = new float[_bones.Length];
            rowRotationSpeeds = new float[_bones.Length];
            orbitRadiusses = new float[_bones.Length];

            for (int i = 0; i < _bones.Length; i++)
            {
                var _orbitSpeed = orbitSpeed.GetRandom() * 0.5f;
                var _orbitRadius = orbitRadius.GetRandom() * (i + 1) / (float)_bones.Length * 2;

                rowAngleOffsets[i] = Random.Range(0f, Mathf.PI * 4f);
                orbitRadiusses[i] = _orbitRadius;
                rowRotationSpeeds[i] = _orbitSpeed * (i - _bones.Length);
            }
        }
    }

    public void SetBones(GameObject[][] bones)
    {
        _bones = bones;
    }

    public void ExecuteFixedUpdate()
    {
        if (_bones == null || _bones.Length == 0) return;

        for (int i = 0; i < _bones.Length; i++)
        {
            int boneCount = _bones[i].Length;
            if (boneCount == 0) continue;

            orbitRadiusses[i] += Mathf.Sin(Time.time) * 0.01f;
            rowAngleOffsets[i] += rowRotationSpeeds[i] * Mathf.Deg2Rad * Time.fixedDeltaTime;

            for (int j = 0; j < boneCount; j++)
            {
                if (_bones[i][j] == null) continue;

                float angle = rowAngleOffsets[i] + j * Mathf.PI * 2f / boneCount;

                Vector3 targetOffset = new Vector3(
                    Mathf.Cos(angle) * orbitRadiusses[i],
                    0,
                    Mathf.Sin(angle) * orbitRadiusses[i]
                );

                Vector3 targetPos = bossTransform.position + targetOffset;

                Rigidbody rb = _bones[i][j].GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.MovePosition(targetPos);
                }
                else
                {
                    _bones[i][j].transform.position = targetPos;
                }
            }
        }
    }

    public void Reset()
    {
        if (_bones == null) return;

        for (int i = 0; i < _bones.Length; i++)
        {
            rowAngleOffsets[i] = Random.Range(0f, Mathf.PI * 4f);
            orbitRadiusses[i] = orbitRadius.GetRandom() * (i + 1) / (float)_bones.Length * 2;
            rowRotationSpeeds[i] = orbitSpeed.GetRandom() * 0.5f * (i - _bones.Length);
        }
    }
}