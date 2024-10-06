using System.IO;
using UnityEngine;
using TMPro;

public class MicRecorder : MonoBehaviour
{
    public GameObject loadingSymbol;
    public TextMeshProUGUI recordingDurationText;

    private AudioClip audioClip;
    private string microphoneName;
    private bool isRecording = false;
    private float recordingStartTime;
    private float recordingDuration; // The actual duration of the recorded audio
    public float RecordingDuration => recordingDuration;

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
            return;
        }
        if (!this.isRecording)
        {
            // Start recording from the microphone
            this.audioClip = Microphone.Start(microphoneName, false, 60, 16000); // 1 minute max with 16kHz sample rate
            this.isRecording = true;
            recordingStartTime = Time.time; // Record the start time
            SetLoadingSymbolVisibility(true);
            Debug.Log("Recording started.");
        }
    }

    public void StopRecording()
    {
        if (this.isRecording)
        {
            // Calculate the actual duration of the recording
            recordingDuration = Time.time - recordingStartTime;

            // Stop recording
            Microphone.End(microphoneName);
            this.isRecording = false;
            SetLoadingSymbolVisibility(false);

            // Trim the audio clip to only the actual recorded length
            if (recordingDuration < 60f)
            {
                TrimRecording();
            }

            Debug.Log("Recording stopped.");
        }
    }

    private void TrimRecording()
    {
        // Get the actual recorded samples (based on time)
        int samplesRecorded = (int)(audioClip.frequency * recordingDuration);
        float[] samples = new float[samplesRecorded];
        audioClip.GetData(samples, 0);

        // Create a new AudioClip with only the trimmed data
        AudioClip trimmedClip = AudioClip.Create(audioClip.name, samplesRecorded, audioClip.channels, audioClip.frequency, false);
        trimmedClip.SetData(samples, 0);

        // Replace the old audioClip with the trimmed one
        audioClip = trimmedClip;

        Debug.Log($"Recording trimmed to {recordingDuration} seconds.");
    }

    private void SetLoadingSymbolVisibility(bool isVisible)
    {
        if (loadingSymbol != null)
        {
            loadingSymbol.SetActive(isVisible);
        }
    }

    public void SaveRecording()
    {
        if (this.audioClip != null && !this.isRecording)
        {
            // Save the recording as a .wav file
            string filePath = Path.Combine(Application.persistentDataPath, "recording.wav");
            WavUtility.SaveWav(filePath, audioClip);
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
