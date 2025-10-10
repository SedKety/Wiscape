using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public enum RainState
{
    Idle,
    MovingBigBones,
    SpawningSmallBones,
    HidingBigBones,
    Waiting,
    DroppingSmallBones,
    Impact,
    SpawningBigWarnings,
    BigWarningWait,
    EmergingBigBones,
    LoweringBigBones
}

public class SkeletonBossRainAttack : MonoBehaviour
{
    [Header("Rain Settings")]
    [SerializeField] private int smallBoneCount = 3;
    [SerializeField] private float rainSpeed = 5f;
    [SerializeField] private float rainRadius = 10f;
    [SerializeField] private float smallBoneRadius = 2f;
    [SerializeField] private float rainWaitTime = 2f;
    [SerializeField] private float boneMoveSpeed = 5f;
    [SerializeField] private float arrivalDistance = 2f;
    [SerializeField] private float bigBoneHeight = 10f;
    [SerializeField] private float smallBoneVerticalSpread = 1f;
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private float bigWarningScale = 1.5f;
    [SerializeField] private float emergeSpeed = 5f;
    [SerializeField] private float loweringTime = 1f;
    [SerializeField] private float impactDelay = 2f;
    [SerializeField] private float bigWarningDelay = 1.5f;
    [SerializeField] private int poolSize = 50;
    [SerializeField] private float undergroundOffset = 1f;

    [Header("Orbit Transition Settings")]
    [SerializeField] private Vector2 orbitRadiusRange = new Vector2(5f, 10f); // Matches SkeletonBossOrbitAttack's orbitRadius
    [SerializeField] private Vector2 orbitSpeedRange = new Vector2(1f, 3f); // Matches SkeletonBossOrbitAttack's orbitSpeed (unused in positioning)

    private GameObject[][] _bones;
    private GameObject[] bonePrefabs;
    private GameObject[] smallBonePrefabs;
    private SpawnBox boneSpawnBox;
    private Transform bossTransform;
    private Vector3 originalWarningScale = Vector3.one;
    private RainState currentRainState = RainState.Idle;
    private List<SmallBoneData> smallBoneData = new List<SmallBoneData>();
    private Dictionary<GameObject, BoneTargetData> bigBoneTargets = new Dictionary<GameObject, BoneTargetData>();
    private List<Vector3> groupLandingCenters = new List<Vector3>();
    private List<GameObject> currentBigWarnings = new List<GameObject>();
    private float rainPhaseStartTime;
    private float loweringStartTime;
    private bool allBigBonesArrived = false;
    private float emergeStartTime;
    private Dictionary<GameObject, Vector3> boneStartPositions = new Dictionary<GameObject, Vector3>();
    private Queue<GameObject> smallBonePool = new Queue<GameObject>();
    private Queue<GameObject> warningPool = new Queue<GameObject>();
    private bool isAttackComplete = false;
    private float[] rowAngleOffsets;
    private float[] orbitRadiusses;
    private bool orbitPositionsInitialized = false;

    public void Initialize(GameObject[][] bones, GameObject[] bonePrefabs, GameObject[] smallBonePrefabs, SpawnBox spawnBox)
    {
        _bones = bones;
        this.bonePrefabs = bonePrefabs;
        this.smallBonePrefabs = smallBonePrefabs;
        this.boneSpawnBox = spawnBox;
        this.bossTransform = transform;

        if (warningPrefab != null)
        {
            originalWarningScale = warningPrefab.transform.localScale;
        }

        if (_bones != null)
        {
            rowAngleOffsets = new float[_bones.Length];
            orbitRadiusses = new float[_bones.Length];
        }

        InitializePools();
    }

    public void SetBones(GameObject[][] bones)
    {
        _bones = bones;
    }

    public RainState GetCurrentRainState()
    {
        return currentRainState;
    }

    public bool IsAttackComplete()
    {
        return isAttackComplete;
    }

    public void Reset()
    {
        currentRainState = RainState.Idle;
        isAttackComplete = false;
        bigBoneTargets.Clear();
        smallBoneData.Clear();
        groupLandingCenters.Clear();
        currentBigWarnings.Clear();
        boneStartPositions.Clear();
        orbitPositionsInitialized = false;
    }

    private void InitializePools()
    {
        if (smallBonePrefabs != null && smallBonePrefabs.Length > 0)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject smallBone = Instantiate(smallBonePrefabs[Random.Range(0, smallBonePrefabs.Length)]);
                smallBone.SetActive(false);
                smallBonePool.Enqueue(smallBone);
            }
        }

        if (warningPrefab != null)
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject warning = Instantiate(warningPrefab);
                warning.SetActive(false);
                warningPool.Enqueue(warning);
            }
        }
    }

    private GameObject GetFromSmallBonePool()
    {
        if (smallBonePool.Count == 0)
        {
            GameObject smallBone = Instantiate(smallBonePrefabs[Random.Range(0, smallBonePrefabs.Length)]);
            return smallBone;
        }
        GameObject pooled = smallBonePool.Dequeue();
        pooled.SetActive(true);
        pooled.transform.localScale = Vector3.one;
        return pooled;
    }

    private void ReturnToSmallBonePool(GameObject smallBone)
    {
        if (smallBone != null)
        {
            smallBone.SetActive(false);
            smallBonePool.Enqueue(smallBone);
        }
    }

    private GameObject GetFromWarningPool()
    {
        if (warningPool.Count == 0)
        {
            GameObject warning = Instantiate(warningPrefab);
            return warning;
        }
        GameObject pooled = warningPool.Dequeue();
        pooled.SetActive(true);
        pooled.transform.localScale = originalWarningScale;
        return pooled;
    }

    private void ReturnToWarningPool(GameObject warning)
    {
        if (warning != null)
        {
            warning.SetActive(false);
            warningPool.Enqueue(warning);
        }
    }

    private void StartRainAttack()
    {
        currentRainState = RainState.MovingBigBones;
        bigBoneTargets.Clear();
        allBigBonesArrived = false;
        smallBoneData.Clear();
        groupLandingCenters.Clear();
        currentBigWarnings.Clear();
        boneStartPositions.Clear();
        isAttackComplete = false;
        orbitPositionsInitialized = false;

        foreach (var row in _bones)
        {
            foreach (var bone in row)
            {
                if (bone == null) continue;

                bone.transform.parent = null;

                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = false;
                }

                Vector3 randomOffset = Random.insideUnitSphere * rainRadius;
                randomOffset.y = 0f;
                Vector3 sampleCenter = bossTransform.position + randomOffset;
                NavMeshHit hit;
                BoneTargetData targetData;
                if (NavMesh.SamplePosition(sampleCenter, out hit, rainRadius, NavMesh.AllAreas))
                {
                    targetData.groundPos = hit.position;
                    targetData.upperPos = hit.position + Vector3.up * bigBoneHeight;
                }
                else
                {
                    targetData.groundPos = sampleCenter;
                    targetData.upperPos = sampleCenter + Vector3.up * bigBoneHeight;
                }
                bigBoneTargets[bone] = targetData;
            }
        }
    }

    public void ExecuteUpdate()
    {
        switch (currentRainState)
        {
            case RainState.Idle:
                // Handled in FixedUpdate
                break;
            case RainState.SpawningSmallBones:
                SpawnSmallBonesAtBigPositions();
                currentRainState = RainState.HidingBigBones;
                break;
            case RainState.HidingBigBones:
                HideBigBones();
                currentRainState = RainState.Waiting;
                rainPhaseStartTime = Time.time;
                break;
            case RainState.Waiting:
                MoveSmallBonesToOffsets();
                if (Time.time - rainPhaseStartTime >= rainWaitTime)
                {
                    currentRainState = RainState.DroppingSmallBones;
                }
                break;
            case RainState.DroppingSmallBones:
                DropSmallBones();
                currentRainState = RainState.Impact;
                break;
            case RainState.SpawningBigWarnings:
                SpawnBigWarnings();
                currentRainState = RainState.BigWarningWait;
                rainPhaseStartTime = Time.time;
                break;
            case RainState.BigWarningWait:
                if (Time.time - rainPhaseStartTime >= bigWarningDelay)
                {
                    currentRainState = RainState.EmergingBigBones;
                    emergeStartTime = Time.time;
                }
                break;
        }
    }

    public void ExecuteFixedUpdate()
    {
        switch (currentRainState)
        {
            case RainState.Idle:
                StartRainAttack();
                break;
            case RainState.MovingBigBones:
                MoveBigBonesToTargets();
                break;
            case RainState.EmergingBigBones:
                EmergeBigBones();
                break;
            case RainState.LoweringBigBones:
                LowerBigBonesToOrbit();
                break;
        }
    }

    private void MoveBigBonesToTargets()
    {
        if (allBigBonesArrived) return;

        bool allArrivedThisFrame = true;
        foreach (var kvp in bigBoneTargets)
        {
            GameObject bone = kvp.Key;
            if (bone == null) continue;

            BoneTargetData targetData = kvp.Value;
            Vector3 target = targetData.upperPos;
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            if (rb == null) continue;

            float distanceToTarget = Vector3.Distance(bone.transform.position, target);
            if (distanceToTarget > arrivalDistance)
            {
                Vector3 direction = (target - bone.transform.position).normalized;
                rb.linearVelocity = direction * boneMoveSpeed;
                allArrivedThisFrame = false;
            }
            else
            {
                rb.linearVelocity = Vector3.zero;
            }
        }

        if (allArrivedThisFrame)
        {
            allBigBonesArrived = true;
            foreach (var kvp in bigBoneTargets)
            {
                GameObject bone = kvp.Key;
                if (bone == null) continue;
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
            currentRainState = RainState.SpawningSmallBones;
        }
    }

    private void SpawnSmallBonesAtBigPositions()
    {
        smallBoneData.Clear();
        groupLandingCenters.Clear();
        Dictionary<GameObject, BoneTargetData> updatedBigBoneTargets = new Dictionary<GameObject, BoneTargetData>();

        foreach (var kvp in bigBoneTargets)
        {
            GameObject bone = kvp.Key;
            if (bone == null) continue;
            Vector3 bigPos = bone.transform.position;
            List<Vector3> smallBoneLandingPositions = new List<Vector3>();

            for (int k = 0; k < smallBoneCount; k++)
            {
                Vector3 spawnOffset = Vector3.zero;
                switch (k)
                {
                    case 0:
                        spawnOffset = Vector3.down * smallBoneVerticalSpread;
                        break;
                    case 1:
                        spawnOffset = Vector3.zero;
                        break;
                    case 2:
                        spawnOffset = Vector3.up * smallBoneVerticalSpread;
                        break;
                }
                Vector3 spawnPos = bigPos + spawnOffset;
                GameObject smallBone = GetFromSmallBonePool();
                smallBone.transform.position = spawnPos;
                smallBone.transform.rotation = Quaternion.identity;

                Rigidbody rb = smallBone.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = smallBone.AddComponent<Rigidbody>();
                }
                rb.isKinematic = true;
                rb.useGravity = false;

                Vector3 horizontalOffset = Random.onUnitSphere * smallBoneRadius;
                horizontalOffset.y = 0f;
                Vector3 targetPos = spawnPos + horizontalOffset;

                GameObject warning = null;
                Vector3 landingPos = Vector3.zero;
                if (warningPrefab != null)
                {
                    warning = GetFromWarningPool();
                    Vector3 rayStart = targetPos + Vector3.up * 10f;
                    if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit rayHit, Mathf.Infinity))
                    {
                        landingPos = rayHit.point;
                        NavMeshHit navHit;
                        if (NavMesh.SamplePosition(landingPos, out navHit, 2f, NavMesh.AllAreas))
                        {
                            landingPos = navHit.position;
                        }
                        warning.transform.position = new Vector3(landingPos.x, landingPos.y + 0.1f, landingPos.z);
                        warning.transform.rotation = Quaternion.identity;
                        warning.transform.localScale = originalWarningScale;
                    }
                    else
                    {
                        landingPos = new Vector3(targetPos.x, 0f, targetPos.z);
                        warning.transform.position = new Vector3(landingPos.x, landingPos.y + 0.1f, landingPos.z);
                        warning.transform.rotation = Quaternion.identity;
                        warning.transform.localScale = originalWarningScale;
                    }
                }

                smallBoneData.Add(new SmallBoneData
                {
                    bone = smallBone,
                    startPos = spawnPos,
                    targetPos = targetPos,
                    warning = warning,
                    landingPos = landingPos
                });

                smallBoneLandingPositions.Add(landingPos);
            }

            Vector3 center = Vector3.zero;
            foreach (var pos in smallBoneLandingPositions)
            {
                center += pos;
            }
            center /= smallBoneCount;
            groupLandingCenters.Add(center);

            BoneTargetData targetData = bigBoneTargets[bone];
            targetData.groundPos = center;
            targetData.upperPos = center + Vector3.up * bigBoneHeight;
            updatedBigBoneTargets[bone] = targetData;
        }

        foreach (var kvp in updatedBigBoneTargets)
        {
            bigBoneTargets[kvp.Key] = kvp.Value;
        }
    }

    private void MoveSmallBonesToOffsets()
    {
        float progress = Mathf.Clamp01((Time.time - rainPhaseStartTime) / rainWaitTime);
        foreach (var data in smallBoneData)
        {
            if (data.bone != null)
            {
                data.bone.transform.position = Vector3.Lerp(data.startPos, data.targetPos, progress);
            }
        }
    }

    private void HideBigBones()
    {
        foreach (var row in _bones)
        {
            if (row == null) continue;
            foreach (var bone in row)
            {
                if (bone != null)
                {
                    bone.SetActive(false);
                }
            }
        }
    }

    private void DropSmallBones()
    {
        foreach (var data in smallBoneData)
        {
            if (data.bone != null)
            {
                Rigidbody rb = data.bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.linearVelocity = Vector3.down * rainSpeed;
                }
            }
        }

        StartCoroutine(HandleImpactAndPostDrop());
    }

    private IEnumerator HandleImpactAndPostDrop()
    {
        yield return new WaitForSeconds(impactDelay);

        foreach (var data in smallBoneData)
        {
            ReturnToSmallBonePool(data.bone);
            if (data.warning != null)
            {
                ReturnToWarningPool(data.warning);
            }
        }
        smallBoneData.Clear();

        currentRainState = RainState.SpawningBigWarnings;
    }

    private void SpawnBigWarnings()
    {
        currentBigWarnings.Clear();
        int centerIndex = 0;
        Dictionary<GameObject, BoneTargetData> updatedBigBoneTargets = new Dictionary<GameObject, BoneTargetData>();

        foreach (var kvp in bigBoneTargets)
        {
            GameObject bone = kvp.Key;
            if (bone == null) continue;

            Vector3 center = groupLandingCenters[centerIndex];
            if (warningPrefab != null)
            {
                GameObject bigWarning = GetFromWarningPool();
                bigWarning.transform.position = new Vector3(center.x, center.y + 0.1f, center.z);
                bigWarning.transform.rotation = Quaternion.identity;
                bigWarning.transform.localScale = new Vector3(originalWarningScale.x * bigWarningScale, originalWarningScale.y, originalWarningScale.z * bigWarningScale);
                currentBigWarnings.Add(bigWarning);
            }

            Vector3 undergroundPos = new Vector3(center.x, center.y - undergroundOffset, center.z);
            bone.transform.position = undergroundPos;
            bone.SetActive(true);
            Rigidbody rb = bone.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
            }

            BoneTargetData targetData = bigBoneTargets[bone];
            targetData.groundPos = center;
            targetData.upperPos = center + Vector3.up * bigBoneHeight;
            updatedBigBoneTargets[bone] = targetData;

            centerIndex++;
        }

        foreach (var kvp in updatedBigBoneTargets)
        {
            bigBoneTargets[kvp.Key] = kvp.Value;
        }

        currentRainState = RainState.BigWarningWait;
        rainPhaseStartTime = Time.time;
    }

    private void EmergeBigBones()
    {
        bool allEmerged = true;
        foreach (var kvp in bigBoneTargets)
        {
            GameObject bone = kvp.Key;
            if (bone == null) continue;

            BoneTargetData targetData = kvp.Value;
            Vector3 targetUpper = targetData.upperPos;
            Rigidbody rb = bone.GetComponent<Rigidbody>();

            float distanceToTarget = Vector3.Distance(bone.transform.position, targetUpper);
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                Vector3 newPos = Vector3.MoveTowards(bone.transform.position, targetUpper, emergeSpeed * Time.fixedDeltaTime);
                rb.MovePosition(newPos);

                if (distanceToTarget > arrivalDistance)
                {
                    allEmerged = false;
                }
                else
                {
                    rb.MovePosition(targetUpper);
                }
            }
            else
            {
                bone.transform.position = Vector3.MoveTowards(bone.transform.position, targetUpper, emergeSpeed * Time.fixedDeltaTime);
                if (distanceToTarget > arrivalDistance)
                {
                    allEmerged = false;
                }
            }
        }

        if (allEmerged)
        {
            currentRainState = RainState.LoweringBigBones;
            loweringStartTime = Time.time;
            boneStartPositions.Clear();
            foreach (var row in _bones)
            {
                if (row == null) continue;
                foreach (var bone in row)
                {
                    if (bone != null)
                    {
                        boneStartPositions[bone] = bone.transform.position;
                    }
                }
            }
        }
    }

    private void LowerBigBonesToOrbit()
    {
        float elapsedTime = Time.time - loweringStartTime;
        float progress = Mathf.Clamp01(elapsedTime / loweringTime);
        bool allLowered = progress >= 1f;

        // Initialize orbit positions to match SkeletonBossOrbitAttack's Reset
        if (!orbitPositionsInitialized)
        {
            for (int i = 0; i < _bones.Length; i++)
            {
                float baseRadius = Random.Range(orbitRadiusRange.x, orbitRadiusRange.y);
                float radius = baseRadius * (i + 1) / (float)_bones.Length * 2;
                orbitRadiusses[i] = Mathf.Max(radius, 1f); // Ensure non-zero radius
                rowAngleOffsets[i] = Random.Range(0f, Mathf.PI * 4f);
                Debug.Log($"Row {i}: radius={orbitRadiusses[i]}, angleOffset={rowAngleOffsets[i]}");
            }
            orbitPositionsInitialized = true;
        }

        int rowIndex = 0;
        bool allAtTarget = true;

        foreach (var row in _bones)
        {
            if (row == null || row.Length == 0)
            {
                rowIndex++;
                continue;
            }

            float radius = orbitRadiusses[rowIndex];
            float angleOffset = rowAngleOffsets[rowIndex];
            int boneCount = row.Length;

            int boneIndexInRow = 0;
            foreach (var bone in row)
            {
                if (bone == null || !boneStartPositions.ContainsKey(bone)) continue;

                // Calculate target orbit position to match SkeletonBossOrbitAttack
                float angle = angleOffset + boneIndexInRow * Mathf.PI * 2f / boneCount;
                Vector3 targetOffset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );
                Vector3 targetOrbitPos = bossTransform.position + targetOffset;

                // Smoothly interpolate to target orbit position in world space
                Vector3 startPos = boneStartPositions[bone];
                Vector3 newPos = Vector3.Lerp(startPos, targetOrbitPos, progress);
                Rigidbody rb = bone.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.MovePosition(newPos);
                }
                else
                {
                    bone.transform.position = newPos;
                }

                float distanceToTarget = Vector3.Distance(bone.transform.position, targetOrbitPos);
                if (distanceToTarget > 0.1f)
                {
                    allAtTarget = false;
                }

                Debug.Log($"Bone {bone.name} (Row {rowIndex}, Index {boneIndexInRow}): targetOrbitPos={targetOrbitPos}, currentPos={bone.transform.position}");

                boneIndexInRow++;
            }
            rowIndex++;
        }

        if (allLowered || allAtTarget)
        {
            foreach (var row in _bones)
            {
                if (row == null) continue;
                foreach (var bone in row)
                {
                    if (bone != null)
                    {
                        bone.transform.parent = bossTransform;
                        Rigidbody rb = bone.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                            rb.useGravity = false;
                            rb.MovePosition(bone.transform.position);
                        }
                    }
                }
            }

            foreach (var w in currentBigWarnings)
            {
                ReturnToWarningPool(w);
            }
            currentBigWarnings.Clear();

            isAttackComplete = true;
            currentRainState = RainState.Idle;
            boneStartPositions.Clear();
            orbitPositionsInitialized = false;
        }
    }
}