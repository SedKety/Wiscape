using UnityEngine;

public class StoolBehaviour : MonoBehaviour
{
    [SerializeField] private CrystalKind kind;
    [SerializeField] private Transform crystalPosition;
    private Transform _placedCrystal;
    private bool _isPlaced;
    public void HandleStool(Transform crystal, Transform overworldCrystal)
    {
        if (_placedCrystal != null) return;
        _placedCrystal = overworldCrystal;
        _placedCrystal.position = crystalPosition.position;

        if (crystal.GetComponent<PuzzlePickUpLogic>().kind == kind)
        {
            _isPlaced = true;
        }
    }

    public bool IsChecked()
    {
        return _isPlaced;
    }

    private void Update()
    {
        if (_isPlaced && _placedCrystal == null)
        {
            _isPlaced = false;
        }
    }
}
