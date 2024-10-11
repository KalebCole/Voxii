using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System;

public class SentimentAnalyzer
{
    private GroqApiClient groqApi;

    public SentimentAnalyzer(bool useMock = false)
    {
        groqApi = new GroqApiClient(useMockForScoring: useMock);
    }

    private JObject GetSystemPrompt(string message)
    {
        return new JObject
        {
            ["role"] = "system",
            ["content"] = $@"
                You are an expert in sentiment analysis. Analyze the sentiment of the following message and categorize it as positive, negative, or neutral. Only respond with one of these three words.

                Message: {message}

                Sentiment:"
        };
    }

    public async Task<bool> IsPositiveOrNeutralSentiment(string message)
    {
        try
        {
            JObject request = new JObject
            {
                ["model"] = "llama-3.1-8b-instant",
                ["messages"] = new JArray { GetSystemPrompt(message) },
                ["max_tokens"] = 10,
                ["temperature"] = 0
            };

            JObject? response = await groqApi.CreateChatCompletionAsync(request);
            var sentiment = response?["choices"]?[0]?["message"]?["content"]?.ToString().Trim().ToLower();

            Debug.Log($"Sentiment for message: {sentiment}");

            return sentiment == "positive" || sentiment == "neutral";
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during sentiment analysis: {ex.Message}");
            return true; // Default to true (positive/neutral) in case of error
        }
    }
}