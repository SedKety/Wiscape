using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    public bool isPuzzleDoor;
    [SerializeField] private Animator animator;
    private bool _hasUsedDoor;

    public void SetPuzzleDoor()
    {
        isPuzzleDoor = true;
    }
    public void UseDoor()
    {
        if (_hasUsedDoor || isPuzzleDoor) return;
        _hasUsedDoor = true;
        animator.SetTrigger("PlayDoorAnimation");
    }

    public void SolvePuzzle()
    {
        animator.SetTrigger("PlayDoorAnimation");
    }
}
