using System.Collections;
using UnityEngine;

public class LockBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public bool isLocked;
    private bool _hasInteracted;

    private void Awake()
    {
        isLocked = true;
    }
    public void UnlockDoor()
    {
        if (_hasInteracted) return;
        _hasInteracted = true;
        animator.SetTrigger("PlayAnimation");
        StartCoroutine(UnlockDoorDelay());
    }

    private IEnumerator UnlockDoorDelay()
    {
        yield return new WaitForSeconds(1.5f);
        isLocked = false;

    }
}
