using UnityEngine;

public class WinTrigger : MonoBehaviour
{
    private bool _hasTriggered;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_hasTriggered)
        {
            UIScreenLogic.Instance.WinGame();
            _hasTriggered = true;

        }
    }
}
