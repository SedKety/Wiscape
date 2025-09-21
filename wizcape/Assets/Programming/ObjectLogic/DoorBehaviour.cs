using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{

    [SerializeField] private Animator animator;
    public void UseDoor()
    {
        animator.SetTrigger("PlayDoorAnimation");
    }
}
