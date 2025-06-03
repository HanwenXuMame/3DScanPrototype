using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap; // Make sure Leap namespace is included

public class ObjectManipulation : MonoBehaviour
{
    private PinchActions pinchActions;
    public GameObject currentObject;
    private Transform initialTransform;

    [Header("Scaling Limits")]
    public float minScale = 0.1f;
    public float maxScale = 3f;
    private Hand leftHand;
    private Hand rightHand;

    private float lastDistance = -1f;
    public LeapProvider leapProvider;
    private Vector3 leftPinchPosition;
    private Vector3 rightPinchPosition;

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
        if (currentObject == null)
        {
            Debug.LogError("Current object is not set in PinchActions.");
            return;
        }
        initialTransform = currentObject.transform;
    }

    void Update()
    {
       if(pinchActions.leftIndexWasPinching && pinchActions.rightIndexWasPinching)
        {
            // Call the scale function with the squish values
            ScaleObject();
        }
        else
        {
            // Reset tracking when pinching stops
            ResetScaleTracking();
        }
    }

    // Call this function every frame while both index fingers are pinching
    public void ScaleObject()
    {
        if (leftHand == null || rightHand == null)
        {
            Debug.LogError("Hands not detected. Ensure Leap Motion is set up correctly.");
            return;
        }


        Debug.Log("Left Pinch Position: " + leftPinchPosition);
        Debug.Log("Right Pinch Position: " + rightPinchPosition);
        float currentDistance = Vector3.Distance(leftPinchPosition, rightPinchPosition);

        if (lastDistance < 0f)
        {
            lastDistance = currentDistance;
            return;
        }

        float scaleChange = (currentDistance - lastDistance) * 1.5f; // Sensitivity factor
        Vector3 newScale = currentObject.transform.localScale + Vector3.one * scaleChange;

        // Clamp scale
        float clamped = Mathf.Clamp(newScale.x, minScale, maxScale);
        newScale = new Vector3(clamped, clamped, clamped);

        currentObject.transform.localScale = newScale;

        lastDistance = currentDistance;
    }

    // Call this when scaling stops (e.g., when either finger releases)
    public void ResetScaleTracking()
    {
        lastDistance = -1f;
    }

    public void ResetObjectPosition(float squish)
    {
        if (currentObject != null)
        {
            currentObject.transform.position = initialTransform.position;
            currentObject.transform.rotation = initialTransform.rotation;
            currentObject.transform.localScale = initialTransform.localScale;
        }
    }
    public void ResetObjectRotation(float squish)
    {
        if (currentObject != null)
        {
            currentObject.transform.rotation = initialTransform.rotation;
        }
    }
}
