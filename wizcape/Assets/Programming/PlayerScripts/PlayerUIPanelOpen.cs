using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerUIPanelOpen : MonoBehaviour
{
    [SerializeField] private Transform cam;
    [SerializeField] private GameObject panel;

    [SerializeField] private LayerMask weaponLayer;
    [SerializeField] private float maxDistance;
    private bool _isActive;

    //Checks if a player is looking at an interactable object and shows UI when able.
    private void Update()
    {
        if (!_isActive && Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, weaponLayer))
        {
            _isActive = true;
            panel.SetActive(true);
        }

        else if (_isActive && !Physics.Raycast(cam.position, cam.forward, out RaycastHit hitAgain, maxDistance, weaponLayer))
        {
            _isActive = false;
            panel.SetActive(false);
        }
    }
}
