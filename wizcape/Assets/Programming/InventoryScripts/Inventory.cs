using NUnit.Framework.Constraints;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance;
    [Header("Inventory")]
    [SerializeField] private List<Item> inventoryItems;
    private Transform inventoryOrbit;

    [Header("CircleStuff")]
    [SerializeField] private List<Transform> inventoryObjects = new List<Transform>();
    [SerializeField] private float radiusAcceleration;
    [SerializeField] private float _currentRadius;
    [SerializeField] private float endRadius;

    [SerializeField] private float startSpeed;
    [SerializeField] private float endSpeed;
    [SerializeField] private float deceleration;

    [SerializeField] private float _currentRotatingSpeed;
    private float _angleOffset;

    private bool _isInventoryOpened;
    [Header("Interacting")]
    [SerializeField] private Transform cam;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask itemLayer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void InventoryInput(InputAction.CallbackContext context)
    {
        if (context.started && !_isInventoryOpened)
        {
            OpenInventory();
        }

        else if (context.started && _isInventoryOpened)
        {
            CloseInventory();
        }
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CheckForItem();
        }        
    }
    private void OpenInventory()
    {
        if (inventoryItems.Count > 0)
        {
            SpawnItems();
            SetValuesToDefault();
            _isInventoryOpened = true;
            
            
        }
    }

    private void SetValuesToDefault()
    {
        _currentRotatingSpeed = startSpeed;
        _currentRadius = 0;
    }

    private void CloseInventory()
    {
        DespawnItems();
    }

    private void DespawnItems()
    {
        _isInventoryOpened = false;
        foreach (Transform item in inventoryObjects)
        {
            Destroy(item.gameObject);
        }

        inventoryObjects.Clear();
    }

    private void Update()
    {
        if (_isInventoryOpened)
        {
            CircleAroundPlayer();
        }
    }
    private void SpawnItems()
    {
        inventoryOrbit = new GameObject().transform;
        inventoryOrbit.position = transform.position;

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            GameObject itemObject = Instantiate(inventoryItems[i].gameObject, inventoryOrbit.position, Quaternion.identity);
            inventoryObjects.Add(itemObject.transform);
        }

        _isInventoryOpened = true;
    }

    private void CircleAroundPlayer()
    {

        if (_currentRotatingSpeed > endSpeed)
        {
            _currentRotatingSpeed -= deceleration * Time.deltaTime;

            if (_currentRotatingSpeed < endSpeed)
            {
                _currentRotatingSpeed = endSpeed;
            }
        }

        if (_currentRadius < endRadius)
        {
            _currentRadius += radiusAcceleration * Time.deltaTime;
        }
        for (int i = 0; i < inventoryObjects.Count; i++)
        {
            float angle = _angleOffset + i * Mathf.PI * 2f / inventoryObjects.Count;

            Vector3 position = new Vector3(
                Mathf.Cos(angle) * _currentRadius,
                0,
                Mathf.Sin(angle) * _currentRadius);

            Vector3 worldPosition = inventoryOrbit.position + position;

            inventoryObjects[i].position = worldPosition;

        }

        _angleOffset += _currentRotatingSpeed * Mathf.Deg2Rad * Time.deltaTime;
    }
    
    //Interacting

    private void CheckForItem()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, maxDistance, itemLayer))
        {
            hitInfo.transform.GetComponent<OverworldItem>().HandleGettingAdded();
        
        }
    }


    public void AddItem(Item item)
    {
        inventoryItems.Add(item);

    }
}
