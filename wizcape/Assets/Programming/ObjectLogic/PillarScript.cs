using System.Collections;
using UnityEngine;

public class PillarScript : MonoBehaviour
{
    [SerializeField] private GameObject fireball;
    [SerializeField] private float minInterval;
    [SerializeField] private float maxInterval;

    private void Start()
    {
        StartCoroutine(HandleFireballFiring());
    }

    private IEnumerator HandleFireballFiring()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minInterval, maxInterval));
            ExecuteFireball();
        }
    }

    private void ExecuteFireball()
    {
        Instantiate(fireball, transform.position, transform.rotation);
    }

}
