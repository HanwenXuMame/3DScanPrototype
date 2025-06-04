using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Root of the entire 3D scene to scale and move")]
    public Transform sceneRoot;

    [Tooltip("The point to zoom toward (e.g. hand/controller position)")]
    public Transform zoomTarget;

    [Header("Zoom Settings")]
    [Tooltip("Maximum scale multiplier for zooming in")]
    public float maxScaleMultiplier = 3f;

    [Range(0f, 1f)]
    [Tooltip("Zoom factor from 0 (default scale) to 1 (maximum zoom)")]
    public float zoomFactor = 0f;

    [SerializeField]
    private Vector3 initialScale;
    [SerializeField]
    private Vector3 initialPosition;
    private bool initialized = false;
    public Camera mainCamera;

    void Start()
    {
        if (sceneRoot == null || zoomTarget == null)
        {
            Debug.LogError("SceneZoomController: Assign sceneRoot and zoomTarget.");
            enabled = false;
            return;
        }

        // Store initial state
        initialScale = sceneRoot.localScale;
        initialPosition = sceneRoot.position;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        ApplyZoom();
    }

void ApplyZoom()
{
    float targetScale = Mathf.Lerp(1f, maxScaleMultiplier, zoomFactor);
    Vector3 newScale = initialScale * targetScale;

    // The point you want to stay visually fixed
    Vector3 pivot = zoomTarget.position;

    // Offset from the pivot to the sceneRoot's original position
    Vector3 offset = initialPosition - pivot;

    // Scale the offset
    Vector3 scaledOffset = offset * targetScale;

    // New position so that zoomTarget remains visually fixed
    Vector3 newPosition = pivot + scaledOffset;

    // Apply transformations
    sceneRoot.localScale = newScale;
    sceneRoot.position = newPosition;

    // Debug
    //Debug.Log($"ZoomFactor: {zoomFactor}, Scale: {newScale}, Pos: {newPosition}");
}
}