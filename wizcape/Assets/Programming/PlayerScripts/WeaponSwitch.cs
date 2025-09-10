using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitch : MonoBehaviour
{
    [SerializeField] private AttackSystem rightHandAttackSystem;
    [SerializeField] private AttackSystem leftHandAttackSystem;
    [SerializeField] private Transform cam;
    [SerializeField] private float raycastMaxDistance;
    [SerializeField] private LayerMask weaponLayer;

    public void RightHandInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CheckForWeapon(rightHandAttackSystem);
        }
    }

    public void LeftHandInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CheckForWeapon(leftHandAttackSystem);
        }
    }

    private void CheckForWeapon(AttackSystem attackSystem)
    {
        print("Checked for a weapon");
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, raycastMaxDistance, weaponLayer))
        {
            print("Found a weapon");
            WeaponHandler weapon = hit.transform.GetComponent<WeaponHandler>();

            weapon.GetPickedUp(attackSystem.transform);
        }
    }
}
