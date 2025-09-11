using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] private GameObject handWeapon;
    private WeaponHandler _currentWeapon;

    private void Start()
    {
        MakeDefaultWeapon();
    }

    private void MakeDefaultWeapon()
    {
        Transform handWeaponClone = Instantiate(handWeapon).transform;
        HandleNewWeapon(handWeaponClone.GetComponent<WeaponHandler>());
    }
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentWeapon.UseAttack();
        }
    }   

    public void HandleNewWeapon(WeaponHandler weapon)
    {
        if (_currentWeapon == null)
        {
            Transform weaponTransform = weapon.transform;
            weaponTransform.position = transform.position;
            weaponTransform.rotation = transform.rotation;
            weaponTransform.parent = transform;
            _currentWeapon = weapon;
        }

        
    }
}
