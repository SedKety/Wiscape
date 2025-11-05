using UnityEngine;

public class BossRoom : MonoBehaviour
{
    [SerializeField] private BossController skeletonBoss;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerEntity>())
        {
            print("Player engaged in battle");
            skeletonBoss.StartBossBehavior();
        }
    }
}
