// Adapted from https://github.com/jgravelle/GroqApiLibrary/blob/master/GroqApiClient.cs

#nullable enable

using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using CandyCoded.env;

public class GroqApiClient
{
    private const string BaseUrl = "https://api.groq.com/openai/v1";
    private const string ChatCompletionsEndpoint = "/chat/completions";

    private string _apiKey;

    public GroqApiClient(string apiKey = "")
    {
        if (apiKey == "" && !env.TryParseEnvironmentVariable("GROQ_API_KEY", out apiKey))
        {
            throw new Exception("API key not provided or error finding environment variable GROQ_API_KEY");
        }
        _apiKey = apiKey;
    }

    public async Task<JObject?> CreateChatCompletionAsync(JObject request)
    {
        string url = BaseUrl + ChatCompletionsEndpoint;
        string jsonData = request.ToString();  // Convert JObject to string

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + _apiKey);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();  // Await until the request is done
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                return JObject.Parse(webRequest.downloadHandler.text);
            }
            else
            {
                throw new Exception($"Error: {webRequest.error}");
            }
        }
    }
}
