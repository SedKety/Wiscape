using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpManager : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Transform cam; //Camera
    [SerializeField] private float maxDistance; //Max Distance
    [SerializeField] private LayerMask pickUpLayer;
    [SerializeField] private float inputInterval;//The layer that can be picked up
    private bool _hasPressedInput;

    [Header("Weapons")]
    [SerializeField] private GameObject handWeapon; //The default hand for attacking.
    private PickUpHandler _currentPickUp; //Currently holding this pick up.
    private bool _isUsingHand; //If true, not holding a weapon.

    [Header("Drop Logic")]
    [SerializeField] private float dropDistance; //The distance that something is dropped from you.



    private void Start()
    {
        MakeDefaultWeapon();
    }

    //Makes the hands for you.
    private void MakeDefaultWeapon()
    {
        _isUsingHand = true;
        Transform handWeaponClone = Instantiate(handWeapon).transform;
        HandleNewPickUp(handWeaponClone.GetComponent<PickUpHandler>(), true);
    }

    //Checks if the attack input has been pressed.
    public void AttackInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _currentPickUp.UsePickUp();
        }
    }   

    //Checks if the interaction is pressed.
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started && !_hasPressedInput)
        {
            StartCoroutine(HandleInputInterval());
            FindWeapon();
        }
    }

    //Checks if there is a weapon.
    private void FindWeapon()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, pickUpLayer))
        {     
            if (hit.transform.GetComponent<OverworldPickUp>() != null)
            {
                
                if (!_isUsingHand && !hit.transform.GetComponent<OverworldPickUp>().IsInInventory()) //If holding a weapon, it drops the current one.
                {
                    DropWeapon();
                }

                else if (hit.transform.GetComponent<OverworldPickUp>().IsInInventory()) //If getting weapon from inventory, it puts the weapon you are holding at that time back in the inventory.
                {
                    HandleInventoryPickUpSwitch(hit.transform.GetComponent<OverworldPickUp>());
                }
                hit.transform.GetComponent<OverworldPickUp>().GetPickedUp(transform); //Handles the pick up
            }
        }

        else
        {
            DropWeapon(); //Drops the weapon.
        }
    }
    public void HandleNewPickUp(PickUpHandler pickUp, bool isHands) //Gets a new pick up
    {
        Transform pickUpTransform = pickUp.transform;
        pickUpTransform.position = transform.position;
        pickUpTransform.rotation = transform.rotation;
        pickUpTransform.parent = transform;

        if (_currentPickUp != null)
        {
            Destroy(_currentPickUp.gameObject);
        }

        _isUsingHand = isHands;

        _currentPickUp = pickUp;
    }

    private void DropWeapon() //Drops the weapon and makes the default hands again.
    {

        if (_isUsingHand) return;
        GameObject overworldPickUp = _currentPickUp.ReturnOverworldPickUp();

        GameObject overworldPickUpClone = Instantiate(overworldPickUp, transform.position, cam.rotation);

        overworldPickUpClone.GetComponent<OverworldPickUp>().HandleDrop();
        MakeDefaultWeapon();
    }

    private void HandleInventoryPickUpSwitch(OverworldPickUp pickedUpWeapon) //Switches the inventory weapon with the weapon you are currently holding.
    {

        if (_isUsingHand)
        {
            Inventory.Instance.HandlePickUp(pickedUpWeapon.gameObject);
        }

        else
        {
            GameObject overworldPickUp = _currentPickUp.ReturnInventoryPickUp();

            GameObject overworldPickUpClone = Instantiate(overworldPickUp, transform.position, Quaternion.identity);

            Inventory.Instance.HandlePickUpSwitch(overworldPickUp, overworldPickUpClone, pickedUpWeapon.gameObject);
        }
        


    }

    private IEnumerator HandleInputInterval()
    {
        _hasPressedInput = true;
        yield return new WaitForSeconds(inputInterval);
        _hasPressedInput = false;
    }

    
}
