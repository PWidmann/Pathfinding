using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("GameInterface reference")]
    [SerializeField] GameInterface gameInterface;

    Camera cam;

    //Zoom
    float cameraSmoothRate = 0.06f;
    private float cameraZoomRate = 8f;
    private float currentCameraSize;
    private float targetCameraSize;

    // Drag
    private Vector3 newPosition;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    float movementRate = 10f;

    private void Start()
    {
        cam = Camera.main;
        targetCameraSize = cam.orthographicSize;
        newPosition = transform.position;
    }

    void LateUpdate()
    {
        HandleCameraZoom();
        HandleCameraDragging();
    }

    void HandleCameraZoom()
    {
        // Camera zoom
        if (Input.GetAxis("Mouse ScrollWheel") != 0f)
        {
            targetCameraSize += Input.GetAxis("Mouse ScrollWheel") * cameraZoomRate * -1;
            targetCameraSize = Mathf.Clamp(targetCameraSize, 2, 150);
        }
        currentCameraSize = cam.orthographicSize;
        cam.orthographicSize = Mathf.Clamp(Mathf.Lerp(currentCameraSize, targetCameraSize, cameraSmoothRate), 2, 100);
    }

    void HandleCameraDragging()
    {
        // Start click position
        if (Input.GetMouseButtonDown(0) && !gameInterface.isSettingStart && !gameInterface.isSettingEnd && !gameInterface.isDrawingWalls)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
                dragStartPosition = ray.GetPoint(entry);
        }

        // Update drag position
        if (Input.GetMouseButton(0) && !gameInterface.isSettingStart && !gameInterface.isSettingEnd && !gameInterface.isDrawingWalls)
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float entry;

            if (plane.Raycast(ray, out entry))
                dragCurrentPosition = ray.GetPoint(entry);

            newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            newPosition.y = transform.position.y;
        }

        // Move camera
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementRate);
    }
}
