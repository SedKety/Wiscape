using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    [SerializeField] private Animator animator;
    private bool _hasUsedDoor;
    public void UseDoor()
    {
        if (_hasUsedDoor) return;
        _hasUsedDoor = true;
        animator.SetTrigger("PlayDoorAnimation");
    }
}
