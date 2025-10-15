using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;


public class SkeletonBossSkeletonSpawner : MonoBehaviour
{
    [SerializeField] private SpawnBox[] skeletonSpawnBoxes;
    [SerializeField] private LayerMask spawnLayer;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private int skeletonCount = 5;
    [SerializeField] private float riseHeight = 2f;
    [SerializeField] private float riseSpeed = 3f;
    [SerializeField] private float undergroundOffset = 1f;
    [SerializeField] private int poolSize = 20;

    private List<GameObject> _activeSkeletons = new List<GameObject>();
    private Queue<GameObject> _skeletonPool = new Queue<GameObject>();
    private Vector3 _origin;
    private Dictionary<GameObject, Vector3> _skeletonTargetPositions = new Dictionary<GameObject, Vector3>();
    private bool _isRising = false;

    public bool SkeletonsAlive => _activeSkeletons.Count > 0;

    private void Awake()
    {
        _origin = transform.position;
        InitializePool();
    }

    private void InitializePool()
    {
        if (skeletonPrefab == null) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject skeleton = Instantiate(skeletonPrefab);
            skeleton.SetActive(false);
            _skeletonPool.Enqueue(skeleton);
        }
    }

    private GameObject GetFromPool()
    {
        if (_skeletonPool.Count == 0)
        {
            GameObject skeleton = Instantiate(skeletonPrefab);
            return skeleton;
        }
        GameObject pooled = _skeletonPool.Dequeue();
        pooled.SetActive(true);
        pooled.transform.localScale = Vector3.one;
        return pooled;
    }

    private void ReturnToPool(GameObject skeleton)
    {
        if (skeleton != null)
        {
            skeleton.SetActive(false);
            _skeletonPool.Enqueue(skeleton);
        }
    }

    public void SpawnSkeletons(int count = -1)
    {
        if (count <= 0) count = skeletonCount;
        _skeletonTargetPositions.Clear();
        _activeSkeletons.Clear();
        _isRising = true;

        for (int i = 0; i < count; i++)
        {
            if (skeletonSpawnBoxes.Length == 0) continue;

            Vector3 spawnPos = skeletonSpawnBoxes[Random.Range(0, skeletonSpawnBoxes.Length)].SpawnPointNavmesh(_origin, spawnLayer);
            Vector3 undergroundPos = spawnPos - Vector3.up * undergroundOffset;
            Vector3 targetPos = spawnPos + Vector3.up * riseHeight;

            GameObject skeleton = GetFromPool();
            skeleton.transform.position = undergroundPos;
            skeleton.transform.rotation = Quaternion.identity;

            // Disable Rigidbody during rising to prevent physics interference
            Rigidbody rb = skeleton.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }

            _activeSkeletons.Add(skeleton);
            _skeletonTargetPositions[skeleton] = targetPos;
        }

        StartCoroutine(RiseSkeletons());
    }

    private IEnumerator RiseSkeletons()
    {
        bool allReached = false;
        while (!allReached && _isRising)
        {
            allReached = true;
            foreach (var skeleton in _activeSkeletons)
            {
                if (skeleton == null) continue;

                Vector3 targetPos = _skeletonTargetPositions[skeleton];
                float distance = Vector3.Distance(skeleton.transform.position, targetPos);
                if (distance > 0.1f)
                {
                    allReached = false;
                    // Use transform-based movement instead of Rigidbody
                    skeleton.transform.position = Vector3.MoveTowards(
                        skeleton.transform.position,
                        targetPos,
                        riseSpeed * Time.deltaTime
                    );
                }
                else
                {
                    skeleton.transform.position = targetPos;
                    // Re-enable Rigidbody physics after rising
                    Rigidbody rb = skeleton.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.isKinematic = false;
                        rb.useGravity = true;
                    }
                }
            }
            yield return null;
        }

        _isRising = false;
    }

    public void CleanupSkeletons()
    {
        _isRising = false;
        foreach (var skeleton in _activeSkeletons)
        {
            if (skeleton != null)
            {
                ReturnToPool(skeleton);
            }
        }
        _activeSkeletons.Clear();
        _skeletonTargetPositions.Clear();
    }

    private void OnDrawGizmos()
    {
        if (skeletonSpawnBoxes == null) return;
        foreach (var box in skeletonSpawnBoxes)
        {
            Gizmos.color = box.GizmoColor;
            Gizmos.DrawWireCube(box.BoxPos + transform.position, box.BoxSize);
        }
    }
}