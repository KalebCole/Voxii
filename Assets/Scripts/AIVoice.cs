using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using PiperSharp;

public static class AIVoice
{
    public static async Task Speak(string msg, bool download = false)
    {
        // await SpeakGoogle(msg, download);
        await SpeakPiper(msg);
    }

    private static async Task SpeakPiper(string msg)
    {
        // TODO: make piper return byte array directly and play it instead of saving it to a file
        string modelLoc = Path.Combine(Application.dataPath, "PiperTTS", "models");
        string workingDir = Path.Combine(Application.dataPath, "PiperTTS", "piper");
        string exeLoc = Path.Combine(workingDir, "piper.exe");

        var model = await VoiceModel.LoadModelByKey(modelLoc, "en_US-ryan-low");

        PiperProvider piperModel = new PiperProvider(new PiperConfiguration()
        {
            ExecutableLocation = exeLoc,
            WorkingDirectory = workingDir,

            Model = model,
            UseCuda = false
        });

        var data = await piperModel.InferAsync(msg, AudioOutputType.Wav);

        // var fs = File.OpenWrite("output.wav");
        // fs.Write(data, 0, data.Length);
        // fs.Flush();
        // fs.Close();

        // Open file in persistent data path
        string filePath = Path.Combine(Application.persistentDataPath, "output.wav");
        File.WriteAllBytes(filePath, data);
    }

    private static async Task SpeakGoogle(string msg, bool download)
    {
        GameObject aiVoiceObject = GameObject.Find("AIVoice");

        if (aiVoiceObject == null)
        {
            Debug.LogError("GameObject 'AIVoice' not found in the scene.");
            return;
        }

        AudioSource audioSource = aiVoiceObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = aiVoiceObject.AddComponent<AudioSource>();
        }

        string url = "https://translate.google.com/translate_tts?ie=UTF-8&client=tw-ob&tl=en&q=" + Uri.EscapeDataString(msg);

        try
        {
            if (download)
            {
                // Download and play from file
                string filePath = Path.Combine(Application.persistentDataPath, "output.mp3");
                await DownloadFileAsync(url, filePath);
                await PlayDownloadedAudio(filePath, audioSource);
            }
            else
            {
                // Stream and play directly from URL
                await StreamAndPlayAudio(url, audioSource);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to download or play the audio: " + e.Message);
        }
    }

    private static async Task DownloadFileAsync(string url, string filePath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0");
            request.downloadHandler = new DownloadHandlerFile(filePath);

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // Allow Unity's main thread to continue while the file downloads.
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(request.error);
            }
        }
    }

    private static async Task PlayDownloadedAudio(string filePath, AudioSource audioSource)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + filePath, AudioType.MPEG))
        {
            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // Ensure non-blocking while loading the audio file.
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.Play();
            }
            else
            {
                Debug.LogError("Error loading audio: " + request.error);
            }
        }
    }

    private static async Task StreamAndPlayAudio(string url, AudioSource audioSource)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            request.SetRequestHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:25.0) Gecko/20100101 Firefox/25.0");

            var asyncOperation = request.SendWebRequest();

            while (!asyncOperation.isDone)
            {
                await Task.Yield(); // Ensure non-blocking while streaming the audio.
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                audioSource.clip = DownloadHandlerAudioClip.GetContent(request);
                audioSource.Play();

                // Wait until the audio has finished playing
                while (audioSource.isPlaying)
                {
                    await Task.Yield(); // Yield control back to the Unity main thread while we wait
                }
            }
            else
            {
                Debug.LogError("Error streaming audio: " + request.error);
            }
        }
    }
}
