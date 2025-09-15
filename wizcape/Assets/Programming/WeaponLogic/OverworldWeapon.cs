using System.Runtime.CompilerServices;
using UnityEngine;

public class OverworldWeapon : MonoBehaviour
{

    [SerializeField] private float grabbingMoveSpeed;
    [SerializeField] private float grabbingRotationSpeed;
    [SerializeField] private GameObject weapon;
    [SerializeField] private Rigidbody rb;

    [SerializeField] private float throwSpeed;
    private bool _isGrabbed;
    private Transform _grabbedArm;
    public void GetPickedUp(Transform handTransform)
    {
        _grabbedArm = handTransform;
        _isGrabbed = true;

        GetComponent<Collider>().isTrigger = true;
        rb.useGravity = false;
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
        GameObject weaponClone = Instantiate(weapon, _grabbedArm.position, _grabbedArm.rotation, _grabbedArm);

        _grabbedArm.GetComponent<WeaponManager>().HandleNewWeapon(weaponClone.GetComponent<WeaponHandler>(), false);
        Destroy(gameObject);
    }

    public void HandleDrop()
    {
        rb.useGravity = true;
        GetComponent<Collider>().isTrigger = false;

        rb.AddForce(transform.forward * throwSpeed, ForceMode.Impulse);
    }
}
