using UnityEngine;

public class OverworldItem : MonoBehaviour
{
    public GameObject itemObject;
    public Item itemScript;


    public void HandleGettingAdded()
    {
        Inventory.Instance.AddItem(itemScript);
        Destroy(gameObject);

    }
}
