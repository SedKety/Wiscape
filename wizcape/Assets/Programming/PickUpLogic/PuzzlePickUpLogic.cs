using UnityEngine;

public enum CrystalKind
{
    Blue,
    Green,
    Red
}
public class PuzzlePickUpLogic : PickUpHandler
{ 
    public CrystalKind kind;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask stoolLayer;
    private Transform _cam;

    private void Start()
    {
        _cam = Camera.main.transform;
    }
    public override void UsePickUp()
    {
        CheckForStool();
    }

    private void CheckForStool()
    {
        if (Physics.Raycast(transform.position, _cam.forward, out RaycastHit hitInfo, maxDistance, stoolLayer))
        {
            if (hitInfo.transform.CompareTag("Stool"))
            {
                GameObject overworldPickUp = ReturnOverworldPickUp();

                GameObject overworldPickUpClone = Instantiate(overworldPickUp, transform.position, _cam.rotation);

                overworldPickUpClone.GetComponent<OverworldPickUp>().HandleDrop();

                
                hitInfo.transform.GetComponent<StoolBehaviour>().HandleStool(transform, overworldPickUpClone.transform);
                transform.parent.GetComponent<PickUpManager>().MakeDefaultWeapon();

            }
        }
    }
}
