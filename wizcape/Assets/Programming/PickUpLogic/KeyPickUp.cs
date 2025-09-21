using UnityEngine;

public class KeyPickUp : PickUpHandler
{

    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask doorLayer;
    private Transform _cam;

    private void Awake()
    {
        _cam = Camera.main.transform;
    }
    public override void UsePickUp()
    {
        CheckForDoor();
    }

    private void CheckForDoor()
    {
        if (Physics.Raycast(_cam.position, _cam.forward, out RaycastHit hit, maxDistance, doorLayer)) 
        {

            transform.parent.GetComponent<PickUpManager>().MakeDefaultWeapon();
            hit.transform.GetComponent<DoorBehaviour>().UseDoor();

        }
    }
}
