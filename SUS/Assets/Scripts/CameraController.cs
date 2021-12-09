using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController SharedInstance;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float movementSpeed;

    [SerializeField] private float movementTime;
    [SerializeField] private float rotationAmount;
    [SerializeField] private Vector3 zoomAmount;

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

    private void Awake()
    {
        if (SharedInstance == null)
            SharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleMovementInput();
    }

    void HandleMouseInput()
    {

        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetMouseButtonDown(2))
            rotateStartPosition = Input.mousePosition;

        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            newRotation*= Quaternion.Euler(Vector3.up * (-difference.x /5f));
        }
    }

    void HandleMovementInput()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            newPosition += (transform.forward * movementSpeed);
        
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            newPosition += (transform.forward * -movementSpeed);
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            newPosition += (transform.right * movementSpeed);
        
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            newPosition += (transform.right * -movementSpeed);

        if (Input.GetKey(KeyCode.Q))
            newRotation*= Quaternion.Euler(Vector3.up * rotationAmount);
        if (Input.GetKey(KeyCode.E))
            newRotation*= Quaternion.Euler(Vector3.up * -rotationAmount);

        if (Input.GetKey(KeyCode.R))
        {
            if(newZoom.y>15f && newZoom.z <-35f)
                newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
            newZoom -= zoomAmount;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition =
            Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
