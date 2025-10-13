using System.Collections;
using UnityEngine;

public class SpikesBehaviour : MonoBehaviour
{
    [SerializeField] private float upInterval;
    [SerializeField] private float downInterval;
    [SerializeField] private Vector3 yUpPosition;
    [SerializeField] private Vector3 yDownPosition;
    [SerializeField] private DamageInstance damageInstance;
    private DamageLayer _dl = DamageLayer.Enemy;

    [SerializeField] private float upSpeed;
    [SerializeField] private float downSpeed;



    private void Start()
    {
        StartCoroutine(HandleSpikesMovement());
    }

    private IEnumerator HandleSpikesMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(upInterval);
            yield return StartCoroutine(HandleSpikesDelay(yUpPosition, upSpeed));
            yield return new WaitForSeconds(downInterval);
            yield return StartCoroutine(HandleSpikesDelay(yDownPosition, downSpeed));
        }
    }

    private IEnumerator HandleSpikesDelay(Vector3 position, float speed)
    {
        

        while (transform.localPosition != position)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, position, speed * Time.deltaTime);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageInstance.Execute(other.gameObject, _dl);
        }
    }
}
