using System.IO;
using UnityEngine;

public class MicRecorder : MonoBehaviour
{
    public GameObject loadingSymbol;

    private AudioClip audioClip;
    private string microphoneName;
    private bool isRecording = false;

    void Start()
    {
        // Get the default microphone
        if (Microphone.devices.Length > 0)
        {
            this.microphoneName = Microphone.devices[0]; // Using the first available microphone
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
        if (!this.isRecording)
        {
            // Start recording from the microphone
            this.audioClip = Microphone.Start(microphoneName, false, 60, 16000); // 1 minute max with 16kHz sample rate since whisper requires that
            this.isRecording = true;
            SetLoadingSymbolVisibility(true);
            Debug.Log("Recording started.");
        }
    }

    public void StopRecording()
    {
        if (this.isRecording)
        {
            // Stop recording
            Microphone.End(microphoneName);
            this.isRecording = false;
            SetLoadingSymbolVisibility(false);
            Debug.Log("Recording stopped.");
        }
    }

    private void SetLoadingSymbolVisibility(bool isVisible)
    {
        Debug.Log("set function activated");
        if (loadingSymbol != null)
        {
            Debug.Log(isVisible);
            loadingSymbol.gameObject.SetActive(isVisible);
        }
    }

    public void SaveRecording()
    {
        if (this.audioClip != null && !this.isRecording)
        {
            // TODO: make sure that the name changes every time you save it
            string filePath = Path.Combine(Application.persistentDataPath, "recording.wav");
            WavUtility.SaveWav(filePath, audioClip);
            PlayRecording();
            Debug.Log("Recording saved to " + filePath);
        }
        else
        {
            Debug.Log("No recording available to save.");
        }
    }

    public void PlayRecording()
    {
        if (this.audioClip != null && !this.isRecording)
        {
            // Play the recorded audio
            AudioSource audioSource = GetComponent<AudioSource>(); // used to get a component attached to the same object
            // the audio source object is attached to the same game object
            audioSource.clip = audioClip;
            audioSource.Play();
            Debug.Log("Playing recording.");
        }
    }
}
