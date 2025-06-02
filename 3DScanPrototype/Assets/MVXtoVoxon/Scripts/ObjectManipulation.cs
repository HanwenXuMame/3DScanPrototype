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
    void OnUpdateFrame(Frame frame)
    {
        //Use a helpful utility function to get the first hand that matches the Chirality
        leftHand = frame.GetHand(Chirality.Left);
        rightHand = frame.GetHand(Chirality.Right);
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

    // Call this function every frame while both index fingers are pinching
    public void ScaleObject(float leftSquish, float rightSquish)
    {
        if (leftHand == null || rightHand == null)
            return;

        Vector3 leftPinchPos = leftHand.GetPinchPosition();
        Vector3 rightPinchPos = rightHand.GetPinchPosition();
        float currentDistance = Vector3.Distance(leftPinchPos, rightPinchPos);

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
