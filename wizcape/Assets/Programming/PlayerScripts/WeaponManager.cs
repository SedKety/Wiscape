using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Transform cam; //Camera
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask weaponLayer;

    [Header("Weapons")]
    [SerializeField] private GameObject handWeapon; //The default hand for attacking.
    private WeaponHandler _currentWeapon;
    private bool _isUsingHand;

    [Header("Drop Logic")]
    [SerializeField] private float dropDistance;

    private void Start()
    {
        MakeDefaultWeapon();
    }
    private void MakeDefaultWeapon()
    {
        _isUsingHand = true;
        Transform handWeaponClone = Instantiate(handWeapon).transform;
        HandleNewWeapon(handWeaponClone.GetComponent<WeaponHandler>(), true);
    }
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentWeapon.UseAttack();
        }
    }   

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            FindWeapon();
        }
    }

    private void FindWeapon()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, weaponLayer))
        {
            if (hit.transform.GetComponent<OverworldWeapon>() != null)
            {
                hit.transform.GetComponent<OverworldWeapon>().GetPickedUp(transform);
            }
        }

        else
        {
            DropWeapon();
        }
    }
    public void HandleNewWeapon(WeaponHandler weapon, bool isHands)
    {
        Transform weaponTransform = weapon.transform;
        weaponTransform.position = transform.position;
        weaponTransform.rotation = transform.rotation;
        weaponTransform.parent = transform;

        if (_currentWeapon != null)
        {
            Destroy(_currentWeapon.gameObject);
        }

        _isUsingHand = isHands;

        _currentWeapon = weapon;
    }

    private void DropWeapon()
    {

        if (_isUsingHand) return;
        GameObject overworldWeapon = _currentWeapon.ReturnOverworldWeapon();

        GameObject overworldWeaponClone = Instantiate(overworldWeapon, transform.position, cam.rotation);

        overworldWeaponClone.GetComponent<OverworldWeapon>().HandleDrop();
        MakeDefaultWeapon();
    }

    
}
