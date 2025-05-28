using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderGenerator : MonoBehaviour
{
    public bool updateCollider = false; // Custom bool to control collider updates
    public float generationInterval = 1.0f; // Time interval for updating the collider

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

    private void GenerateBoxCollider()
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
