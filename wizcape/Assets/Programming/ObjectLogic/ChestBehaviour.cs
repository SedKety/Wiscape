using System.Collections;
using UnityEngine;

public class ChestBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject item;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform itemEndPosition;
    [SerializeField] private float itemSpeed;
    private Transform _itemClone;
    private bool _hasOpened;

    public void OpenChest()
    {
        if (_hasOpened) return;

        StartCoroutine(PlayAnimation());
        _hasOpened = true;
        
    }

    private IEnumerator PlayAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("PlayAnimation");
        }

        yield return new WaitForSeconds(1);

        SpawnItem();
    }

    private void SpawnItem()
    {
        _itemClone = Instantiate(item, transform.position, Quaternion.identity).transform;

        StartCoroutine(MoveItemUp());
    }

    private IEnumerator MoveItemUp()
    {
        while (_itemClone.position != itemEndPosition.position)
        {
            
            _itemClone.position = Vector3.Lerp(_itemClone.position, itemEndPosition.position, itemSpeed * Time.deltaTime);
            yield return null;
            if (_itemClone == null)
            {
                break;
            }
        }
    }
}
