using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{

    public static Inventory Instance;
    [Header("Inventory")]
    [SerializeField] private List<GameObject> inventoryPickUps;
    [SerializeField] private int inventorySize;
    private Transform inventoryOrbit;

    [Header("CircleStuff")]
    [SerializeField] private List<Transform> inventoryObjects = new List<Transform>();
    [SerializeField] private List<Transform> inventoryObjectsClone = new List<Transform>();
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
    [SerializeField] private LayerMask overworldPickUpLayer;

    private bool _hasPressedInput;

    [Header("Object choosing")]
    [SerializeField] private Transform endPosition;
    [SerializeField] private float moveSpeed;
    private int _previousIndex;
    private Transform _grabbedObject;



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
            CheckForPickUp();
        }        
    }
    private void OpenInventory()
    {
        if (inventoryPickUps.Count > 0)
        {
            SpawnPickUps();
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
        StartCoroutine(WaitForDespawn());
    }

    private IEnumerator WaitForDespawn()
    {
        _hasPressedInput = true;
        float timer = 0;
        float endTimer = 1;
        while (timer < endTimer)
        {
            _currentRadius -= radiusAcceleration * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        DespawnPickUps();
        _hasPressedInput = false;
    }
    private void DespawnPickUps()
    {
        _isInventoryOpened = false;
        foreach (Transform item in inventoryObjectsClone)
        {
            Destroy(item.gameObject);
        }

        inventoryObjects.Clear();
        inventoryObjectsClone.Clear();
    }

    private void Update()
    {
        if (_isInventoryOpened)
        {
            CircleAroundPlayer(1);
            CheckForInput();
            MoveObjectTowardsPosition();
            CheckIfOutOfRadius();
        }
    }
    private void SpawnPickUps()
    {
        inventoryOrbit = new GameObject().transform;
        inventoryOrbit.position = transform.position;

        for (int i = 0; i < inventoryPickUps.Count; i++)
        {
            GameObject itemObject = Instantiate(inventoryPickUps[i], inventoryOrbit.position, Quaternion.identity);
            inventoryObjects.Add(itemObject.transform);
            inventoryObjectsClone.Add(itemObject.transform);
        }


        _isInventoryOpened = true;
    }

    private void CircleAroundPlayer(int sign)
    {

        if (_currentRotatingSpeed > endSpeed)
        {
            _currentRotatingSpeed -= deceleration * Time.deltaTime;

            if (_currentRotatingSpeed < endSpeed)
            {
                _currentRotatingSpeed = endSpeed;
            }
        }

        if (_currentRadius < endRadius && !_hasPressedInput)
        {
            _currentRadius += sign * (radiusAcceleration * Time.deltaTime);
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

    private void CheckForPickUp()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, maxDistance, overworldPickUpLayer))
        {
            if (inventoryPickUps.Count >= inventorySize) return; 
            if (hitInfo.transform.GetComponent<OverworldPickUp>().IsInInventory()) return;
            hitInfo.transform.GetComponent<OverworldPickUp>().PutInInventory();
        
        }
    }

    public void HandlePickUp(GameObject inventoryPickUp)
    {
        int index = inventoryObjects.IndexOf(inventoryPickUp.transform);
        inventoryObjects.Remove(inventoryPickUp.transform);

        inventoryPickUps.RemoveAt(index);
    }

    public void HandlePickUpSwitch(GameObject currentPickUpPrefab, GameObject currentPickUp,  GameObject inventoryPickUp)
    {
        int index = inventoryObjects.IndexOf(inventoryPickUp.transform);
        inventoryObjects.Remove(inventoryPickUp.transform);

        inventoryPickUps.RemoveAt(index);

        inventoryObjects.Insert(index, currentPickUp.transform);
        inventoryPickUps.Insert(index, currentPickUpPrefab);

    }
    public void AddPickUp(GameObject pickUp)
    {
        inventoryPickUps.Add(pickUp);

    }

    public void RemovePickUp(GameObject pickUp)
    {
        inventoryPickUps.Remove(pickUp);

    }

    private void CheckForInput()
    {
        for (int i = 1; i <= 9; i++)
        {

            if (i > inventoryObjects.Count + 1) break;
            KeyCode key = KeyCode.Alpha0 + i;

            if (Input.GetKeyDown(key))
            {
                BringItem(i - 1);
            }
        }
    }

    private void BringItem(int index)
    {
        if (_grabbedObject != null)
        {
            inventoryObjects.Insert(_previousIndex, _grabbedObject);

        }
        _grabbedObject = inventoryObjectsClone[index];

        inventoryObjects.Remove(_grabbedObject);
        _previousIndex = index;

    }

    private void MoveObjectTowardsPosition()
    {
        if (_grabbedObject == null) return;

        _grabbedObject.position = Vector3.Lerp(_grabbedObject.position, endPosition.position, moveSpeed * Time.deltaTime);
    }

    private void CheckIfOutOfRadius()
    {
        if (Vector3.Distance(transform.position, inventoryOrbit.position) > 2)
        {
            CloseInventory();
        }
    }


}
