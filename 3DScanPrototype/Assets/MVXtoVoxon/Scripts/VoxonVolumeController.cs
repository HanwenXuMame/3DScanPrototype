using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxonVolumeController : MonoBehaviour
{
    public VXCaptureVolume captureVolume;
    public Vector3 volumeScale = new Vector3(0.1f, 0.1f, 0.1f);

    // Start is called before the first frame update
    void Start()
    {
        if (captureVolume == null)
        {
            captureVolume = GetComponent<VXCaptureVolume>();
        }

        // Start the coroutine to set the vector scale
        StartCoroutine(SetCaptureVolumeScaleAtEndOfFrame());
    }

    // Coroutine to set the capture volume scale at the end of the frame
    private IEnumerator SetCaptureVolumeScaleAtEndOfFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (captureVolume != null)
            {
                captureVolume.vectorScale = volumeScale;
            }
        }
    }
}
