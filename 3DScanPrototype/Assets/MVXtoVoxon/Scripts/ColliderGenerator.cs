using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour
{
    public bool updateCollider = false; // Custom bool to control collider updates
    public float generationInterval = 1.0f; // Time interval for updating the collider
    public BoxCollider boundingBox;

    private GameObject meshPart; // Reference to the child object named "MeshPart"
    private BoxCollider generatedCollider; // Reference to the generated BoxCollider
    private float timer = 0.0f; // Timer to track interval

    void Start()
    {
        // Try to find the child object named "MeshPart" in the hierarchy
        meshPart = FindChildRecursive(transform, "MeshPart");

        if (meshPart != null)
        {
            // Generate the initial box collider if MeshPart is found
            GenerateBoxCollider();
        }
    }

    void Update()
    {
        // If MeshPart is not found, keep checking for it
        if (meshPart == null)
        {
            meshPart = FindChildRecursive(transform, "MeshPart");

            if (meshPart != null)
            {
                Debug.Log("MeshPart found! Generating initial collider.");
                GenerateBoxCollider();
            }

            return; // Skip the rest of the update logic until MeshPart is found
        }

        // If updateCollider is true, update the collider every generationInterval seconds
        if (updateCollider)
        {
            timer += Time.deltaTime;
            if (timer >= generationInterval)
            {
                GenerateBoxCollider();
                timer = 0.0f;
            }
        }
    }

    public void GenerateBoxCollider()
    {
        // Get the MeshFilter from MeshPart
        MeshFilter meshFilter = meshPart.GetComponent<MeshFilter>();
        if (meshFilter == null || meshFilter.sharedMesh == null)
        {
            Debug.LogError("MeshPart does not have a MeshFilter or a valid Mesh!");
            return;
        }

        // Add a temporary BoxCollider to MeshPart
        BoxCollider tempCollider = meshPart.AddComponent<BoxCollider>();

        // Calculate the bounding box of the mesh
        Bounds meshBounds = meshFilter.sharedMesh.bounds;
        tempCollider.center = meshBounds.center;
        tempCollider.size = meshBounds.size;

        // If a collider already exists on this object, destroy it
        if (generatedCollider != null)
        {
            Destroy(generatedCollider);
        }

        // Add a new BoxCollider to this object
        generatedCollider = gameObject.AddComponent<BoxCollider>();

        // Copy the properties from the temporary BoxCollider
        generatedCollider.center = tempCollider.center;
        generatedCollider.size = tempCollider.size;

        // Destroy the temporary BoxCollider on MeshPart
        Destroy(tempCollider);

        // --- New logic: Fit object inside bounding box ---

        if (boundingBox != null)
        {
            // Get world size of generated collider
            Vector3 objSize = Vector3.Scale(generatedCollider.size, transform.lossyScale);
            Vector3 boundingSize = Vector3.Scale(boundingBox.size, boundingBox.transform.lossyScale);

            // Find the scale factor needed to fit the largest dimension
            float scaleX = boundingSize.x / objSize.x;
            float scaleY = boundingSize.y / objSize.y;
            float scaleZ = boundingSize.z / objSize.z;
            float minScale = Mathf.Min(scaleX, scaleY, scaleZ, 1f); // Don't upscale

            // Apply uniform scaling if needed
            if (minScale < 1f)
            {
                transform.localScale *= minScale;
                // Recalculate collider size after scaling
                objSize = Vector3.Scale(generatedCollider.size, transform.lossyScale);
            }

            // Now reposition so the object is inside the bounding box
            // Get world bounds of object and bounding box
            Bounds objWorldBounds = generatedCollider.bounds;
            Bounds boundingWorldBounds = boundingBox.bounds;

            Vector3 offset = Vector3.zero;

            // For each axis, move object so it's inside the bounding box
            for (int i = 0; i < 3; i++)
            {
                float objMin = objWorldBounds.min[i];
                float objMax = objWorldBounds.max[i];
                float boundMin = boundingWorldBounds.min[i];
                float boundMax = boundingWorldBounds.max[i];

                if (objMin < boundMin)
                    offset[i] += boundMin - objMin;
                if (objMax > boundMax)
                    offset[i] += boundMax - objMax;
            }

            // Move the object by the offset
            transform.position += offset;
        }
    }

    // Recursive function to find a child object by name
    private GameObject FindChildRecursive(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }

            GameObject found = FindChildRecursive(child, childName);
            if (found != null)
            {
                return found;
            }
        }

        return null;
    }
}
