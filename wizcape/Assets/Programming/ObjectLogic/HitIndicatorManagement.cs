using System.Collections;
using UnityEngine;

public class HitIndicatorManagement : MonoBehaviour
{
    private bool _isHit;
    [SerializeField] private float timeInterval;

    public void StartHitIndicator()
    {
        StartCoroutine(GetHit());
    }

    private IEnumerator GetHit()
    {
        _isHit = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.red;
        }

        yield return new WaitForSeconds(timeInterval);

        foreach (Renderer renderer in renderers)
        {
            renderer.material.color = Color.white;
        }

        _isHit = false;
    }

    public bool CheckIfBeingHit()
    {
        return _isHit;
    }
}
