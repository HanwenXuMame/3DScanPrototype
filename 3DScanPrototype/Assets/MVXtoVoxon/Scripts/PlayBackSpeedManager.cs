using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MVXUnity;

public class PlayBackSpeedManager : MonoBehaviour
{
    public bool isPaused = false;
    [Range(0.1f, 3.0f)]
    public float playbackSpeed = 1.0f; // Default playback speed
    private float currentPlaybackSpeed = 1.0f; // Current playback speed

    private MvxAudioPlayerStream audioPlayerStream;

    void Start()
    {
        // Find the MvxAudioPlayerStream object in the scene at the end of the first frame
        StartCoroutine(FindAudioPlayerStream());
    }

    IEnumerator FindAudioPlayerStream()
    {
        // Wait until the end of the first frame
        yield return null;

        // Find the MvxAudioPlayerStream component in the scene
        audioPlayerStream = FindObjectOfType<MvxAudioPlayerStream>();

        if (audioPlayerStream == null)
        {
            Debug.LogError("MvxAudioPlayerStream component not found in the scene.");
        }
        else
        {
            // Set the playback speed
            audioPlayerStream.playbackSpeed = playbackSpeed;
            Debug.Log($"Playback speed set to {playbackSpeed}");
        }
    }

    void Update()
    {
        if (audioPlayerStream == null) return;

        if (isPaused)
        {
            currentPlaybackSpeed = audioPlayerStream.playbackSpeed;
            audioPlayerStream.playbackSpeed = 0f; // Pause the MvxAudioPlayerStream
        }
        else
        {
            audioPlayerStream.playbackSpeed = playbackSpeed; // Resume with the current playback speed
        }
    }

    public void Pause(bool pause)
    {
        isPaused = pause;
        if (isPaused)
        {
            Debug.Log("Playback paused.");
        }
        else
        {
            Debug.Log("Playback resumed.");
        }
    }

    public void IncrementPlaybackSpeed(float increment)
    {
        playbackSpeed += increment;
        if (playbackSpeed > 3.0f) // Limit the maximum playback speed
        {
            playbackSpeed = 3.0f;
        }
        if (playbackSpeed < 0.1f)
        {
            playbackSpeed = 0.1f; // Limit the minimum playback speed
        }
        audioPlayerStream.playbackSpeed = playbackSpeed; // Update the MvxAudioPlayerStream
        Debug.Log($"Playback speed increased to {playbackSpeed}");
    }
}
