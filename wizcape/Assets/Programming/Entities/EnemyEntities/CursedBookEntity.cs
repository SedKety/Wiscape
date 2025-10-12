using EditorAttributes;
using UnityEngine;

public class CursedBookEntity : EnemyEntity
{
    [GUIColor(GUIColor.Purple)]
    [Header("Cursed Book Settings")]

    [Tooltip("The cursed book's transform for hovering")]
    [SerializeField] private Transform cursedBookGO;

    [Tooltip("Min/Max height the book hover's at")]
    [SerializeField] private RandomFloatV2 hoverHeight;

    [Tooltip("The speed of the hover motion")]
    [SerializeField] private float hoverFrequency = 1f;


    [Tooltip("The range or amplitude of the hover motion")]
    [SerializeField] private float hoverRange = 0.5f;


    private float _initialHeight;

    protected override void Awake()
    {
        _initialHeight = transform.position.y;
        cursedBookGO.position = new Vector3(cursedBookGO.position.x, hoverHeight.GetRandom(), cursedBookGO.position.z);
        
        base.Awake();
    }

    private void FixedUpdate()
    {
        float hoverY = hoverHeight.Last + Mathf.Sin(Time.time * hoverFrequency) * hoverRange;
        Vector3 newPosition = new Vector3(cursedBookGO.position.x, hoverY + _initialHeight, cursedBookGO.position.z);
        cursedBookGO.position = newPosition;
    }

}
