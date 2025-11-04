using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class PlayerUIPanelOpen : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text text;
    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private float maxDistance;
    private string _originalText;
    private bool _isActive;
    private bool _isLockedText;

    private void Start()
    {
        _originalText = text.text;
    }
    //Checks if a player is looking at an interactable object and shows UI when able.
    private void Update()
    {
        if (!_isActive && Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, weaponLayer))
        {
            if (hit.transform.TryGetComponent(out LockBehaviour lockBehaviour))
            {
                if (lockBehaviour.IsLocked()) text.text = "Locked. You need a key!";
                _isLockedText = true;
            }  
            _isActive = true;
            panel.SetActive(true);
        }

        else if (_isActive && !Physics.Raycast(cam.position, cam.forward, out RaycastHit hitAgain, maxDistance, weaponLayer))
        {
            if (_isLockedText)
            {
                text.text = _originalText;
                _isLockedText = false;
            }
            _isActive = false;
            panel.SetActive(false);
        }


    }
}
