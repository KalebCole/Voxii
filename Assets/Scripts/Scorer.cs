# nullable enable

using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;

public class Scorer
{
    public GroqApiClient groqApi = new GroqApiClient();

    // Accept a 'useMock' parameter for the Scorer class to enable mock mode (called in SecondaryBtnPress)
    public Scorer(string chatLogFilePath, bool useMockForScoring = false)
    {
        this.chatLogFilePath = chatLogFilePath;
        groqApi = new GroqApiClient(useMockForScoring: useMockForScoring);
    }

    private JObject systemPrompt // this dynamically generates upon instantiation of the scorer class (so we can interpolate the chat log file)
    {
        get
        {
            return new JObject
            {
                ["role"] = "system",
                ["content"] = $@"
                    You are proficient in the English Language.

                    I want you to grade the user in a conversation between a user and an AI avatar based on the following criteria:

                    Number of major grammatical errors in the user's responses (only count significant mistakes such as incorrect verb conjugation, subject-verb agreement errors, and sentence structure issues).
                    Analysis of the relevance and depth of understanding in the user's responses. Gauge how accurately the user understood the assistant's questions or prompts and whether their responses reflect a clear understanding of the conversation context.
                    I also want you to analyze the overall sentiment of the AI avatar's responses throughout the conversation and categorize the sentiment as positive, negative, or neutral based on the general tone and mood expressed by the AI.

                    Here is an example of the expected output format:

                    Number of errors: 2
                    Accuracy of understanding and responding: 8 (scale of 0-10)
                    Sentiment: positive
                    
                    The conversation will be in the following format:
                    01:12:02 User: Hi, how are you?
                    02:13:03 assistant: I am doing well, thank you for asking. How can I help you today?
                    
                    Here is the conversation that you will be grading:

                "
            };
        }
    }
    // 2. Average time for the response in seconds

    private string chatLogFilePath;

    public Scorer(string chatLogFilePath)
    {
        this.chatLogFilePath = chatLogFilePath;
    }

    private JArray GetMsgs()
    {
        // Initialize this with system prompt
        JArray msgs = new JArray
    {
        systemPrompt
    };
        UnityEngine.Debug.Log("msgs with system prompt: " + msgs);

        if (File.Exists(chatLogFilePath))
        {
            string[] messages = File.ReadAllLines(chatLogFilePath);
            UnityEngine.Debug.Log("length: " + messages.Length);
            string result = string.Join("\r\n", messages);
            msgs[0]["content"] += result;
        }
        string expectedOutcome = @"
                                    Output the grades as a string in this format:
                                    Number of errors: [number]
                                    Accuracy of understanding and responding: [number between 0-10]
                                    Sentiment: [positive, negative, or neutral]
                                    Do not output any other text in the response.
                        
        
";
        // Average time for response: number (in seconds),
        msgs[0]["content"] += expectedOutcome;
        UnityEngine.Debug.Log("Msgs after: " + msgs);
        return msgs;
    }

    public async Task<string> GetScore()
    {
        try
        {
            JObject request = new JObject
            {
                ["model"] = "llama-3.1-8b-instant",
                ["messages"] = GetMsgs(),
                ["max_tokens"] = 100,
                ["temperature"] = 0
            };

            UnityEngine.Debug.Log("Request: " + request);
            JObject? response = await groqApi.CreateChatCompletionAsync(request);
            UnityEngine.Debug.Log("after request sent: " + response);

            var content = response?["choices"]?[0]?["message"]?["content"];
            UnityEngine.Debug.Log("content: " + content);

            return content?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Error during API call: " + ex.Message);
            return string.Empty;
        }
    }

}


