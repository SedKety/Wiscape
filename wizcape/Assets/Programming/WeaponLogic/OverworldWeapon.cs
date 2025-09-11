using System.Runtime.CompilerServices;
using UnityEngine;

public class OverworldWeapon : EntityBase
{

    [SerializeField] private float grabbingMoveSpeed;
    [SerializeField] private float grabbingRotationSpeed;
    [SerializeField] private GameObject weapon;
    private bool _isGrabbed;
    private Transform _grabbedArm;
    public void GetPickedUp(Transform handTransform)
    {
        _grabbedArm = handTransform;
        _isGrabbed = true;

        GetComponent<WeaponFloating>().GetGrabbed();
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
        Destroy(gameObject);
    }
}
