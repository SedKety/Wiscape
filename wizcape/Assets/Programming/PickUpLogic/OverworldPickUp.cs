using JetBrains.Annotations;
using NUnit.Framework.Internal.Filters;
using System.Runtime.CompilerServices;
using UnityEngine;

public class OverworldPickUp : MonoBehaviour
{
    [Header("PickUp Variables")]
    [SerializeField] private float grabbingMoveSpeed;
    [SerializeField] private float grabbingRotationSpeed;
    [SerializeField] private GameObject pickUp;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float throwSpeed;

    [SerializeField] private GameObject inventoryObject;
    private bool _isGrabbed;
    private Transform _grabbedArm;
    

    [Header("Inventory Variables")]
    [SerializeField] private bool isInInventory;

    public void GetPickedUp(Transform handTransform)
    {
        if (_isGrabbed) return;
        _grabbedArm = handTransform;
        _isGrabbed = true;

        GetComponent<Collider>().isTrigger = true;

        if (rb != null)
        {
            rb.useGravity = false;

        }
    }

    private void Update()
    {
        if (_isGrabbed)
        {
            MoveTowardsArm();
        }
    }

    private void MoveTowardsArm()
    {
        transform.position = Vector3.Lerp(transform.position, _grabbedArm.position, grabbingMoveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, _grabbedArm.rotation, grabbingRotationSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _grabbedArm.position) < 0.1f)
        {
            HandleWeaponChange();
        }
    }

    private void HandleWeaponChange()
    {
        GameObject weaponClone = Instantiate(pickUp, _grabbedArm.position, _grabbedArm.rotation, _grabbedArm);
        _grabbedArm.GetComponent<PickUpManager>().HandleNewPickUp(weaponClone.GetComponent<PickUpHandler>(), false);
        Destroy(gameObject);
    }

    public void HandleDrop()
    {
        rb.useGravity = true;
        GetComponent<Collider>().isTrigger = false;

        rb.AddForce(transform.forward * throwSpeed, ForceMode.Impulse);
    }

    public void PutInInventory()
    {
        Inventory.Instance.AddPickUp(inventoryObject);
        Destroy(gameObject);

    }

    public bool IsInInventory()
    {
        return isInInventory;
    }

    public GameObject ReturnInventoryPrefab()
    {
        return inventoryObject;
    }
}
