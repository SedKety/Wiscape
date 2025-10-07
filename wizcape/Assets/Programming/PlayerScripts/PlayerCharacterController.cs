using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacterController : MonoBehaviour
{

    //Camera Variables
    [Header("Camera Variables")]
    [SerializeField] private Transform cam;
    [SerializeField] private Vector2 mouseInput;
    [SerializeField] private float sensX, sensY;
    [SerializeField] private Transform walkReference;
    private float _xRotation, _yRotation;

    //Movement Variables
    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    private float _xInput, _yInput;
    private Rigidbody _rb;

    //Headbop Variables
    [Header("Headbop Variables")]
    [SerializeField] private float amount;
    [SerializeField] private float frequency;
    [SerializeField] private float smooth;
    [SerializeField] private Transform hands;
    private Vector3 _startHandPosition;

    //Chest Interact
    [Header("Misc Interact Variables")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask objectLayer;
    [SerializeField] private LayerMask doorLayer;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
    }


    private void Start()
    {
        _startHandPosition = hands.localPosition;
    }
    public void PlayerMovement(InputAction.CallbackContext context)
    {
        _xInput = context.ReadValue<Vector2>().x;
        _yInput = context.ReadValue<Vector2>().y;
    }
    public void MouseMovement(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }

    public void InteractInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            ChestCheck();
            DoorCheck();
        }
    }

    private void FixedUpdate()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        Vector3 walkPosition = walkReference.forward * _yInput * walkSpeed * Time.deltaTime
            + walkReference.right * _xInput * walkSpeed * Time.deltaTime;

        Vector3 newVelocity = new Vector3(walkPosition.x, _rb.linearVelocity.y, walkPosition.z);
        _rb.linearVelocity = newVelocity;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();
        HandleHeadBop();
    }

    private void HandleCameraRotation()
    {
        float xPosition = mouseInput.x * Time.deltaTime * sensX;
        float yPosition = mouseInput.y * Time.deltaTime * sensY;

        _yRotation += xPosition;
        _xRotation -= yPosition;

        _xRotation = Mathf.Clamp(_xRotation, -90, 90);
        cam.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        walkReference.rotation = Quaternion.Euler(0, _yRotation, 0);
    }

    private void HandleHeadBop()
    {
        if (_xInput != 0 || _yInput != 0)
        {
            StartHeadBop();
        }

        else if (hands.localPosition != _startHandPosition)
        {
            StopHeadBop();
        }
    }

    private void StartHeadBop()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
        hands.localPosition += pos;

    }

    private void StopHeadBop()
    {
        hands.localPosition = Vector3.Lerp(hands.localPosition, _startHandPosition, 3 * Time.deltaTime);
    }

    private void ChestCheck()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, objectLayer))
        {
            hit.transform.GetComponent<ChestBehaviour>().OpenChest();
        }
    }

    private void DoorCheck()
    {
        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, maxDistance, doorLayer))
        {
            if (hit.transform.TryGetComponent(out LockBehaviour lockBehaviour))
            {
                if (lockBehaviour.isLocked) return;
                
            }
            hit.transform.GetComponent<DoorBehaviour>().UseDoor();
        }
    }
}
