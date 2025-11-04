using System.Collections;
using UnityEngine;

public class LockBehaviour : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject doorLock;
    public bool isLocked;
    private bool _hasInteracted;

    public void LockDoor()
    {
        isLocked = true;
    }
    
    public void UnlockDoor()
    {
        if (_hasInteracted) return;
        _hasInteracted = true;
        isLocked = false;
    }

    public bool IsLocked()
    {
        return isLocked;
    }

}
