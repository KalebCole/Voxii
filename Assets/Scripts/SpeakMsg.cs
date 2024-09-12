using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class SpeakMsg
{
    public static async void DownloadAndSpeak(string msg)
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
        string filePath = Path.Combine(Application.persistentDataPath, "output.mp3");

        try
        {
            await DownloadFileAsync(url, filePath);
            await PlayDownloadedAudio(filePath, audioSource);
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
}
