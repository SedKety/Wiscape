using System.Collections.Generic;
using UnityEngine;

public class SwordTrail : MonoBehaviour
{
    [SerializeField] private Transform trailTransform;
    [SerializeField] private Transform tip;
    [SerializeField] private Transform baseObject;
    [SerializeField] private int trailLength = 15;
    [SerializeField] private float trailWidth = 0.1f;
    [SerializeField] private float fadeSpeed = 3f;

    private Mesh mesh;
    private List<Vector3> tipPositions = new List<Vector3>();
    private List<Vector3> basePositions = new List<Vector3>();

    void Start()
    {
        mesh = new Mesh();
        trailTransform.GetComponent<MeshFilter>().mesh = mesh;
    }

    void LateUpdate()
    {
        tipPositions.Insert(0, tip.position);
        basePositions.Insert(0, baseObject.position);

        if (tipPositions.Count > trailLength)
        {
            tipPositions.RemoveAt(trailLength);
            basePositions.RemoveAt(trailLength);
        }

        UpdateTrailMesh();
    }

    void UpdateTrailMesh()
    {
        mesh.Clear();

        int segmentCount = tipPositions.Count - 1;
        if (segmentCount < 1) return;

        Vector3[] vertices = new Vector3[segmentCount * 2];
        int[] triangles = new int[(segmentCount - 1) * 6];
        Vector2[] uvs = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < segmentCount; i++)
        {
            vertices[i * 2] = transform.InverseTransformPoint(basePositions[i]);
            vertices[i * 2 + 1] = transform.InverseTransformPoint(tipPositions[i]);

            float t = (float)i / (trailLength - 1);
            uvs[i * 2] = new Vector2(0, t);
            uvs[i * 2 + 1] = new Vector2(1, t);
            colors[i * 2] = colors[i * 2 + 1] = new Color(1, 1, 1, 1 - t * fadeSpeed);
        }

        for (int i = 0; i < segmentCount - 1; i++)
        {
            int vi = i * 2;
            int ti = i * 6;
            triangles[ti] = vi;
            triangles[ti + 1] = vi + 1;
            triangles[ti + 2] = vi + 2;
            triangles[ti + 3] = vi + 1;
            triangles[ti + 4] = vi + 3;
            triangles[ti + 5] = vi + 2;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateNormals();
    }
}
