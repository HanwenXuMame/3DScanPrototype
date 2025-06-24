using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap; // Make sure Leap namespace is included

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
    private Hand leftHand;
    private Hand rightHand;
    public LeapProvider leapProvider;
    private Vector3 leftPinchPosition;
    private Vector3 rightPinchPosition;
    private PinchActions pinchActions;
    private float lastDistance = -1f;
    private bool pinchingStarted = false;
    private float initialPinchDistance = 0f;
    public float zoomSensitivity = 0.01f; // Adjust this value to control zoom speed

    private void OnEnable()
    {
        leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }
    void OnUpdateFrame(Frame frame)
    {

        //Use a helpful utility function to get the first hand that matches the Chirality
        leftHand = frame.GetHand(Chirality.Left);
        rightHand = frame.GetHand(Chirality.Right);
        //When we have a valid left hand, we can begin searching for more Hand information
        if (leftHand != null)
        {
            OnUpdateLeftHand(leftHand);
        }
        if (rightHand != null)
        {
            OnUpdateRightHand(rightHand);
        }
    }

    void OnUpdateLeftHand(Hand _hand)
    {
        float _pinchStrength = _hand.PinchStrength;
        float _pinchDistance = _hand.PinchDistance;
        leftPinchPosition = _hand.GetPinchPosition();
        Vector3 _predictedPinchPosition = _hand.GetPredictedPinchPosition();
        bool isPinching = _hand.IsPinching();// Here we can get additional information.
    }

    void OnUpdateRightHand(Hand _hand)
    {
        float _pinchStrength = _hand.PinchStrength;
        float _pinchDistance = _hand.PinchDistance;
        rightPinchPosition = _hand.GetPinchPosition();
        Vector3 _predictedPinchPosition = _hand.GetPredictedPinchPosition();
        bool isPinching = _hand.IsPinching();// Here we can get additional information.
    }



    void Start()
    {

        pinchActions = FindObjectOfType<PinchActions>();
        if (pinchActions == null)
        {
            Debug.LogError("PinchActions script not found in the scene.");
            return;
        }
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
        if (pinchActions.leftIndexWasPinching && pinchActions.rightIndexWasPinching)
        {
            if (!pinchingStarted)
            {
                SetZoomTarget();
                pinchingStarted = true;
            }
            AdjustZoomFactor();
        }
        else
        {
            ResetScaleTracking();
            pinchingStarted = false;
        }
    }

    void SetZoomTarget()
    {
        zoomTarget.position = (leftPinchPosition + rightPinchPosition) / 2f;
        initialPinchDistance = Vector3.Distance(leftPinchPosition, rightPinchPosition);
    }

    public void AdjustZoomFactor()
    {
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("Hands not detected. Ensure Leap Motion is set up correctly.");
            return;
        }

        // Debugging hand positions
        Debug.Log("Left Pinch Position: " + leftPinchPosition);
        Debug.Log("Right Pinch Position: " + rightPinchPosition);

        // Calculate the current distance between the hands
        float currentDistance = Vector3.Distance(leftPinchPosition, rightPinchPosition);

        // If this is the first frame of tracking, initialize lastDistance
        if (lastDistance < 0f)
        {
            lastDistance = currentDistance;
            return;
        }

        // Calculate the zoom factor based on the change in distance
        float distanceChange = currentDistance - initialPinchDistance;

        // Adjust zoomFactor proportionally to the distance change

        zoomFactor = Mathf.Clamp(zoomFactor + (distanceChange * zoomSensitivity), 0f, 1f);

        // Update lastDistance for the next frame
        lastDistance = currentDistance;
    }

    public void ResetScaleTracking()
    {
        lastDistance = -1f;
    }

    public void ApplyZoom()
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
    
    public void ResetZoom()
    {
        if (!initialized) return;

        zoomFactor = 0f; // Reset zoom factor
        lastDistance = -1f; // Reset distance tracking
        pinchingStarted = false; // Reset pinch state
    }
}