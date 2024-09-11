using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Whisper.net;
using Whisper.net.Logger;

public class WhisperTranscriber : MonoBehaviour
{
    // Variable for the user text display
    public TextMeshProUGUI displayText;

    // Variables to hold the model path and audio file path.
    private string modelPath;
    private string wavFilePath;
    private WhisperProcessor processor;
    private WhisperFactory whisperFactory;
    private bool processorDisposed = false;
    private bool whisperFactoryDisposed = false;


    // Start is called before the first frame update in Unity
    void Start()
    {
        // Set the ggmlType and initialize model and wav file paths
        // var ggmlType = GgmlType.Base;
        this.modelPath = Path.Combine(Application.dataPath, "WhisperModels", "ggml-base.en.bin");
        this.wavFilePath = Path.Combine(Application.persistentDataPath, "recording.wav");

        // Check if the model file exists; if not, download it
        if (!File.Exists(this.modelPath))
        {
            Debug.Log("File doesn't exist");
        }

        Debug.Log("Test");

        this.whisperFactory = WhisperFactory.FromPath(this.modelPath);
        this.processor = this.whisperFactory.CreateBuilder()
                                      .WithLanguage("en")
                                      .Build();

        // Optional: Log messages from the Whisper library
        LogProvider.Instance.OnLog += (level, message) =>
        {
            Debug.Log($"{level}: {message}");
        };

        // Create and use the Whisper factory and processor to transcribe the audio file
        // await ProcessAudio();
    }

    void OnDestroy()
    {
        // Ensure proper cleanup when the script is destroyed
        if (!this.processorDisposed)
        {
            this.processor?.Dispose();
        }
        if (!this.whisperFactoryDisposed)
        {
            this.whisperFactory?.Dispose();
        }
    }

    public async Task<string> TranscribeRecording()
    {
        StringBuilder finalResult = new StringBuilder();

        using var fileStream = File.OpenRead(this.wavFilePath);

        // Process the audio file asynchronously and log the results
        await foreach (var result in this.processor.ProcessAsync(fileStream))
        {
            Debug.Log(result.Text);

            // Update the UI text
            displayText.text = result.Text;

            // Append to final result
            finalResult.AppendLine(result.Text);
        }

        return finalResult.ToString();
    }
}
