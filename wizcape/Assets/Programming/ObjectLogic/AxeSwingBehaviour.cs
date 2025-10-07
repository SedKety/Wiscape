using System.Collections;
using UnityEngine;

public class AxeSwingBehaviour : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float angle;
    [SerializeField] private DamageInstance damageInstance;
    private DamageLayer _dl = DamageLayer.Enemy;

    private float _time;

    private void Start()
    {
        _time = Random.Range(0f, 4f);
    }
    private void Update()
    {
        _time += Time.deltaTime * speed;

        float rotationZ = Mathf.Sin(_time) * angle;

        transform.localRotation = Quaternion.Euler(0, 0, rotationZ);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageInstance.Execute(other.gameObject, _dl);
        }
    }
}
