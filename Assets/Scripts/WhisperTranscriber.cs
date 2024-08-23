using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Whisper.net;
using Whisper.net.Logger;

public class WhisperTranscriber : MonoBehaviour
{
    // Variables to hold the model path and audio file path.
    private string modelPath;
    private string wavFilePath;
    private WhisperProcessor processor;
    private WhisperFactory whisperFactory;


    // Start is called before the first frame update in Unity
    async void Start()
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
        await ProcessAudio();
    }

    void OnDestroy()
    {
        // Ensure proper cleanup when the script is destroyed
        this.processor?.Dispose();
        this.whisperFactory?.Dispose();
    }

    private async Task ProcessAudio()
    {

        using var fileStream = File.OpenRead(this.wavFilePath);

        // Process the audio file asynchronously and log the results
        await foreach (var result in this.processor.ProcessAsync(fileStream))
        {
            Debug.Log($"{result.Start}->{result.End}: {result.Text}");
        }
    }
}