using UnityEngine;

public class PotionPickUp : PickUpHandler
{
    public ItemObject itemScriptableObject;
    public int hpUp;

    private void Awake()
    {
        hpUp = itemScriptableObject.hpUp;
    }
    public override void UsePickUp()
    {
        PlayerEntity.Instance.GetHealth(hpUp);
        transform.parent.GetComponent<PickUpManager>().MakeDefaultWeapon();
    }
}
