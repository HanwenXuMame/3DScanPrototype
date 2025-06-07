using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap; // Make sure Leap namespace is included

public class ObjectManipulation : MonoBehaviour
{

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


    void Start()
    {

        if (currentObject == null)
        {
            Debug.LogError("Current object is not set in PinchActions.");
            return;
        }
        initialTransform = currentObject.transform;
    }

    // Call this function every frame while both index fingers are pinching
    public void ChangeZoomIn()
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
