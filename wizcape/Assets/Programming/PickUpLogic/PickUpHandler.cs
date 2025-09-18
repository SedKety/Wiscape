using UnityEngine;

public class PickUpHandler : MonoBehaviour
{
    public GameObject overWorldPickUp;
    public GameObject inventoryPickUp;
    public virtual void UsePickUp()
    {
        print("Uses PickUp");
    }

    public GameObject ReturnOverworldPickUp()
    {
        return overWorldPickUp;
    }

    public GameObject ReturnInventoryPickUp()
    {
        return inventoryPickUp;
    }
}
