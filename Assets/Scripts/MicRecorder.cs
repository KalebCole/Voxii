using UnityEngine;

public class MicRecorder : MonoBehaviour
{
    private AudioClip audioClip;
    private string microphoneName;
    private bool isRecording = false;

    void Start()
    {
        // Get the default microphone
        if (Microphone.devices.Length > 0)
        {
            microphoneName = Microphone.devices[0]; // Using the first available microphone
        }
        else
        {
            Debug.LogWarning("No microphone found!");
        }
    }

    public void StartRecording()
    {
        if (microphoneName == null)
        {
            Debug.Log("Unable to record. Mic not detected");
        }
        if (!isRecording)
        {
            // Start recording from the microphone
            audioClip = Microphone.Start(microphoneName, false, 60, 44100); // 1 minute max with 16kHz sample rate since whisper requires that
            isRecording = true;
            Debug.Log("Recording started.");
        }
    }

    public void StopRecording()
    {
        if (isRecording)
        {
            // Stop recording
            Microphone.End(microphoneName);
            isRecording = false;
            Debug.Log("Recording stopped.");
        }
    }

    public void PlayRecording()
    {
        if (audioClip != null && !isRecording)
        {
            // Play the recorded audio
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.Play();
            Debug.Log("Playing recording.");
        }
    }
}
