using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using MVXUnity;
using UnityEngine.InputSystem;

public class MVXFileManager : MonoBehaviour
{
    // A public list to store all file names from the StreamingAssets folder
    public List<string> streamingAssetsFileNames = new List<string>();
    public MvxFileDataStreamDefinition mvxFileDataStreamDefinition;
    public int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        if (mvxFileDataStreamDefinition == null)
        {
            Debug.LogError("MvxFileDataStreamDefinition is not assigned in the inspector. Auto assigning...");
            mvxFileDataStreamDefinition = FindObjectOfType<MvxFileDataStreamDefinition>();
            if (mvxFileDataStreamDefinition == null)
            {
                Debug.LogError("No MvxFileDataStreamDefinition found in the scene. Please assign it in the inspector.");
                return;
            }
        }
        // Get the StreamingAssets folder path
        string folderPath = Application.streamingAssetsPath;

        // Check if the folder exists
        if (Directory.Exists(folderPath))
        {
            // Get all files in the folder (non-recursive)
            string[] files = Directory.GetFiles(folderPath);

            // Clear the list, then add each file name if not a .meta or .json file
            streamingAssetsFileNames.Clear();
            foreach (string filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                // Only include .mvx files
                if (extension != ".mvx")
                    continue;

                streamingAssetsFileNames.Add(Path.GetFileName(filePath));
            }

            Debug.Log("Found " + streamingAssetsFileNames.Count + " files in StreamingAssets.");
        }
        else
        {
            Debug.LogError("StreamingAssets folder not found at " + folderPath);
        }
        LoadMVXFile();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            LoopMVXFiles(true); // Move to the next file
            LoadMVXFile(); // Load the new file
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            LoopMVXFiles(false); // Move to the previous file
            LoadMVXFile(); // Load the new file
        }
    }

    // LoopMVXFiles moves the current index up (if direction is true) or down (if false), wrapping around if needed.
    public void LoopMVXFiles(bool direction)
    {
        if (streamingAssetsFileNames.Count == 0)
            return;

        if (direction)
        {
            currentIndex++;
            if (currentIndex >= streamingAssetsFileNames.Count)
                currentIndex = 0;
        }
        else
        {
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = streamingAssetsFileNames.Count - 1;
        }
    }

    // LoadMVXFile sets the file path of mvxFileDataStreamDefinition to the file at the current index.
    public void LoadMVXFile()
    {
        if (streamingAssetsFileNames.Count == 0)
            return;

        string fileName = streamingAssetsFileNames[currentIndex];
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // Assuming mvxFileDataStreamDefinition has a property called "filePath"
        mvxFileDataStreamDefinition.filePath = filePath;
        Debug.Log("Loading MVX file: " + filePath);
    }
}
