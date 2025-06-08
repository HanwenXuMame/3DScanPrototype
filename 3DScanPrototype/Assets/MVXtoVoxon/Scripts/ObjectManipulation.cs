using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManipulation : MonoBehaviour
{
    public GameObject currentObject;
    [SerializeField]private Vector3 initialPosition;
    [SerializeField]private Quaternion initialRotation;
    [SerializeField]private Vector3 initialScale;

    [Header("Test Settings")]
    public bool testResetObjectTransform = false;

    void Start()
    {
        if (currentObject == null)
        {
            Debug.LogError("Current object is not set in PinchActions.");
            return;
        }

        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Current object does not have a Rigidbody component.");
            return;
        }

        // Store the initial world transform values
        initialPosition = rb.position;
        initialRotation = rb.rotation;
        initialScale = currentObject.transform.localScale;

        Debug.Log($"Initial Transform Set: Position={initialPosition}, Rotation={initialRotation}, Scale={initialScale}");
    }

    void Update()
    {
        // Check if the test boolean is true and call ResetObjectTransform
        if (testResetObjectTransform)
        {
            ResetObjectTransform(0f); // Pass a dummy squish value
            testResetObjectTransform = false; // Reset the test boolean
        }
    }

    public void ResetObjectTransform(float squish)
    {
        if (currentObject != null)
        {
            Rigidbody rb = currentObject.GetComponent<Rigidbody>();
            if (rb == null)
            {
                Debug.LogError("Current object does not have a Rigidbody component.");
                return;
            }

            // Reset the Rigidbody's position and rotation
            rb.position = initialPosition;
            rb.rotation = initialRotation;

            // Reset the scale of the object
            currentObject.transform.localScale = initialScale;

            Debug.Log($"Transform Reset: Position={rb.position}, Rotation={rb.rotation}, Scale={currentObject.transform.localScale}");
        }
    }
}
