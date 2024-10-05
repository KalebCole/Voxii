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

    private string[] commonGramaticalErrorsInSpeech = new string[] { "Subject-Verb Agreement", "Verb Conjugation", "Sentence Structure" };

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

                    Number of major grammatical errors in the user's responses, categorized by type. Use the following list of common grammatical errors: {commonGramaticalErrorsInSpeech.Aggregate((i, j) => i + ", " + j)}.
                        For each grammatical error, categorize it based on the type (e.g., subject-verb agreement, verb conjugation, sentence structure).
                        Provide specific examples of the grammatical errors from the user's responses.
                        For each example, provide a corrected version of the sentence as well.

                    Analysis of the relevance and depth of understanding in the user's responses. Gauge how accurately the user understood the assistant's questions or prompts and whether their responses reflect a clear understanding of the conversation context.

                    
                    I also want you to analyze the overall sentiment of the AI avatar's responses throughout the conversation and categorize the sentiment as positive, negative, or neutral based on the general tone and mood expressed by the AI.

                    Here is an example of the expected output format:

                    Number of errors: 3

                    Error Examples: 
                    1. Subject-Verb Agreement:
                    Incorrect: 'She were happy.'
                    Corrected: 'She was happy.'
                    Reasoning: The verb 'were' does not agree with the singular subject 'She.' The correct verb form should be 'was.'
                    2. Sentence Structure:
                    Incorrect: 'I doing well.'
                    Corrected: 'I am doing well.'
                    Reasoning: The sentence lacks the auxiliary verb 'am' to form the present continuous tense.
                    3. Verb Conjugation:
                    Incorrect: 'He go to the store.'
                    Corrected: 'He goes to the store.'
                    Reasoning: The verb 'go' should be conjugated to 'goes' to match the third-person singular subject 'He.'

                    Accuracy of understanding and responding: 8

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
                                Output Format as a string:   
                                    Number of errors: [number]
                                    Error Examples: 
                                    1. [Category of the error]:
                                    Incorrect: '[example of the error]'
                                    Corrected: '[corrected sentence]'
                                    Reasoning: [explanation of the error and correction]

                                    Accuracy of understanding and responding: [number between 0-10]

                                    Sentiment: [positive, negative, or neutral]
                                Do not output any other information or text in the response. The output should be a single string with the format specified above.
                                
                                If there are no errors, the output should be:
                                    Number of errors: 0
                                    Accuracy of understanding and responding: 10
                                    Sentiment: positive
                        
        
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


