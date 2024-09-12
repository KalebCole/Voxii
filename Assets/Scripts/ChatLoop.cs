# nullable enable

using UnityEngine;
using Newtonsoft.Json.Linq;
// using IPA;

public class ChatLoop : MonoBehaviour
{
    static readonly string initialAIMessage = "Hello, welcome to our cafe. What can I get for you today?";
    static readonly JObject systemPrompt = new JObject
    {
        ["role"] = "system",
        ["content"] = "You are a cafe barista. You reply with very short answers."
    };

    public GroqApiClient groqApi = new GroqApiClient();
    private JArray chatHistory = new JArray
    {
        systemPrompt,
        new JObject
        {
            ["role"] = "assistant",
            ["content"] = initialAIMessage
        }
    };


    private void TrimChatHistory(int maxMessages = 5)
    {
        // +1 for the system prompt
        if (chatHistory.Count <= maxMessages * 2 + 1) return;
        chatHistory.RemoveAt(1);
        chatHistory.RemoveAt(1);
    }

    public async void SendUserMessage(string msg)
    {
        Debug.Log("Sending message: " + msg);
        JObject userMessage = new JObject { ["role"] = "user", ["content"] = msg };
        TrimChatHistory();
        chatHistory.Add(userMessage);

        JObject request = new JObject
        {
            ["model"] = "llama-3.1-8b-instant",
            ["messages"] = chatHistory,
            ["max_tokens"] = 100,
            ["temperature"] = 1.2
        };

        JObject? response = await groqApi.CreateChatCompletionAsync(request);
        var content = response?["choices"]?[0]?["message"]?["content"];
        if (content == null)
        {
            Debug.LogError("Error: response content is null");
            return;
        }

        JObject assistantMessage = new JObject
        {
            ["role"] = "assistant",
            ["content"] = content
        };
        chatHistory.Add(assistantMessage);
        Debug.Log("Assistant: " + assistantMessage["content"]);

        // TODO: call tts
        AIVoice.Speak(assistantMessage["content"].ToString());
    }
}
