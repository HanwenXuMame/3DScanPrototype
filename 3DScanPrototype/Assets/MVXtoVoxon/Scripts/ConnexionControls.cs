using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpaceNavigatorDriver;

public class ConnexionControls : MonoBehaviour
{
    public GameObject currentObject;
    public float rotationSpeed;
    public float scalingSpeed;
    public float translationSpeed;
    public KeyCode scaleUpInput;
    public KeyCode scaleDownInput;
    public float minScale = 0.01f;
    public float maxScale = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentObject == null) return;

        Rigidbody rb = currentObject.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Translation (world space, not affected by character rotation)
        Vector3 translation = SpaceNavigatorHID.current.Translation.ReadValue() * translationSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + translation);

        // Rotation (Y axis only, left/right)
        Vector3 rotation = SpaceNavigatorHID.current.Rotation.ReadValue();
        Quaternion deltaRotation = Quaternion.Euler(0f, rotation.y * rotationSpeed * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * deltaRotation);

        // Scaling (still via transform)
        Vector3 scale = currentObject.transform.localScale;
        if (Input.GetKey(scaleUpInput))
        {
            scale += Vector3.one * scalingSpeed * Time.deltaTime;
        }
        if (Input.GetKey(scaleDownInput))
        {
            scale -= Vector3.one * scalingSpeed * Time.deltaTime;
        }
        // Clamp scale
        float clamped = Mathf.Clamp(scale.x, minScale, maxScale);
        currentObject.transform.localScale = new Vector3(clamped, clamped, clamped);
    }
}
