# nullable enable

using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Scorer
{
    public GroqApiClient groqApi = new GroqApiClient();

    private string[] commonGramaticalErrorsInSpeech = new string[] { "Subject-Verb Agreement", "Verb Conjugation", "Sentence Structure" };

    // TODO: improve the error calcuation by accounting for relevance and depth of understanding in the user's responses
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


    private string chatLogFilePath;

    public Scorer(string chatLogFilePath)
    {
        this.chatLogFilePath = chatLogFilePath;
        groqApi = new GroqApiClient(useMockForScoring: false);
    }

    // Accept a 'useMock' parameter for the Scorer class to enable mock mode (called in SecondaryBtnPress)
    public Scorer(string chatLogFilePath, bool useMockForScoring = false)
    {
        this.chatLogFilePath = chatLogFilePath;
        groqApi = new GroqApiClient(useMockForScoring: useMockForScoring);
    }

    // Get the messages between the user and AI avatar from the chat log file and append the system prompt
    // also appends the expected outcome to the chat log
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

    // Get the response output from the AI model and calling the getMsgs method to get the chat log
    public async Task<string> GetResponseOutput()
    {
        try
        {
            JObject request = new JObject
            {
                ["model"] = "llama-3.1-8b-instant",
                ["messages"] = GetMsgs(),
                ["max_tokens"] = 300,
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

    // Overload the GetResponseOutput method to accept a JArray parameter
    // This is useful for testing the scoring system with different chat logs
    public async Task<string> GetResponseOutput(JArray msgs)
    {
        try
        {
            JObject request = new JObject
            {
                ["model"] = "llama-3.1-8b-instant",
                ["messages"] = msgs,
                ["max_tokens"] = 300,
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

    // Parse the score string to extract the number of errors and accuracy
    public ScoreResult ParseScoreNumbers(string scoreString)
    {
        var result = new ScoreResult();

        var errorsMatch = Regex.Match(scoreString, @"Number of errors:\s*(\d+)");
        var accuracyMatch = Regex.Match(scoreString, @"Accuracy of understanding and responding:\s*(\d+)");

        if (errorsMatch.Success)
            result.NumberOfErrors = int.Parse(errorsMatch.Groups[1].Value);

        if (accuracyMatch.Success)
            result.Accuracy = int.Parse(accuracyMatch.Groups[1].Value);


        //debug
        UnityEngine.Debug.Log("Number of errors: " + result.NumberOfErrors);
        UnityEngine.Debug.Log("Accuracy: " + result.Accuracy);
        // UnityEngine.Debug.Log("Average response time: " + result.AverageResponseTime);
        return result;
    }

    // Parse the sentiment from the AI response
    public SentimentResult ParseSentiment(string scoreString)
    {
        var result = new SentimentResult();

        var sentimentMatch = Regex.Match(scoreString, @"Sentiment:\s*(\w+)");
        if (sentimentMatch.Success)
            result.Sentiment = sentimentMatch.Groups[1].Value;

        // debug
        UnityEngine.Debug.Log("Sentiment: " + result.Sentiment);

        return result;
    }

    // Parse the error examples from the AI response
    public List<ErrorExample> ParseErrorExamples(string scoreString)
    {
        var errorExamples = new List<ErrorExample>();

        var errorMatch = Regex.Match(scoreString, @"Error Examples:(.*)Accuracy of understanding and responding:", RegexOptions.Singleline);
        if (errorMatch.Success)
        {
            var errorList = errorMatch.Groups[1].Value;
            var errorLines = errorList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < errorLines.Length; i += 4)
            {
                var error = new ErrorExample
                {
                    Category = errorLines[i].Trim(),
                    Incorrect = errorLines[i + 1].Trim(),
                    Corrected = errorLines[i + 2].Trim(),
                    Reasoning = errorLines[i + 3].Trim()
                };
                errorExamples.Add(error);
            }
        }

        // debug
        UnityEngine.Debug.Log("Error Examples:");
        foreach (var error in errorExamples)
        {
            UnityEngine.Debug.Log(error.Category);
            UnityEngine.Debug.Log(error.Incorrect);
            UnityEngine.Debug.Log(error.Corrected);
            UnityEngine.Debug.Log(error.Reasoning);
        }
        return errorExamples;
    }

    // Calculate the time taken for the user to respond to the AI
    public float CalculateResponseTime(string chatLogFilePath)
    {
        // debug
        UnityEngine.Debug.Log("Calculating response time...");

        float responseTime = 0;
        if (File.Exists(chatLogFilePath))
        {
            string[] messages = File.ReadAllLines(chatLogFilePath);
            if (messages.Length >= 2)
            {
                string[] lastMessage = messages[messages.Length - 2].Split(' ');
                string[] currentTime = messages[messages.Length - 1].Split(' ');
                if (lastMessage.Length >= 2 && currentTime.Length >= 2)
                {
                    string lastTime = lastMessage[0];
                    string currentTimeStr = currentTime[0];
                    if (TimeSpan.TryParse(lastTime, out TimeSpan lastTimeSpan) && TimeSpan.TryParse(currentTimeStr, out TimeSpan currentTimeSpan))
                    {
                        // debug
                        responseTime = (float)(currentTimeSpan - lastTimeSpan).TotalSeconds;
                        UnityEngine.Debug.Log("Response time: " + responseTime);
                        return responseTime;
                    }
                }
            }
        }
        // debug
        UnityEngine.Debug.Log("Response time: " + responseTime);
        return responseTime;
    }

    // Calculate the points based on the score result
    public static int CalculatePoints(ScoreResult scoreResult, float responseTime)
    {
        UnityEngine.Debug.Log("Calculating points...");

        int points = 0;


        // Points for number of errors
        switch (scoreResult.NumberOfErrors)
        {
            case 0:
                points += 100;
                break;
            case 1:
                points += 80;
                break;
            case 2:
                points += 60;
                break;
            case 3:
                points += 40;
                break;
            case 4:
                points += 20;
                break;
            case 5:
                points += 10;
                break;
            default:
                points += 0;
                break;
        }

        // Points for accuracy
        switch (scoreResult.Accuracy)
        {
            case 0:
                points += 0;
                break;
            case 1:
                points += 10;
                break;
            case 2:
                points += 20;
                break;
            case 3:
                points += 30;
                break;
            case 4:
                points += 40;
                break;
            case 5:
                points += 50;
                break;
            case 6:
                points += 60;
                break;
            case 7:
                points += 70;
                break;
            case 8:
                points += 80;
                break;
            case 9:
                points += 90;
                break;
            case 10:
                points += 100;
                break;
            default:
                points += 0;
                break;
        }

        // Points for response time
        if (responseTime <= 5)
        {
            points += 100;
        }
        else if (responseTime <= 10)
        {
            points += 80;
        }
        else if (responseTime <= 15)
        {
            points += 60;
        }
        else if (responseTime <= 20)
        {
            points += 40;
        }
        else if (responseTime <= 25)
        {
            points += 20;
        }
        else if (responseTime <= 30)
        {
            points += 10;
        }
        else
        {
            points += 0;
        }

        // debug
        UnityEngine.Debug.Log("Points: " + points);


        return points;
    }

    // Step by step calculation of points
    public async Task<int> CalculatePointsAsync()
    {
        var messages = GetMsgs();
        var scoreString = await GetResponseOutput(messages);
        var scoreResult = ParseScoreNumbers(scoreString);
        var responseTime = CalculateResponseTime(chatLogFilePath);
        return CalculatePoints(scoreResult, responseTime);
    }

    // Get ScoreResult object
    public async Task<(ScoreResult, float, List<ErrorExample>)> GetResultsAndResponseTimeAsync()
    {
        var messages = GetMsgs();
        var scoreString = await GetResponseOutput(messages);
        var scoreResult = ParseScoreNumbers(scoreString);
        var errors = ParseErrorExamples(scoreString);
        var responseTime = CalculateResponseTime(chatLogFilePath);
        return (scoreResult, responseTime, errors);
    }



}

public class ScoreResult
{
    public int NumberOfErrors { get; set; }
    public int Accuracy { get; set; }
    // public float AverageResponseTime { get; set; }
}

// sentiment result class
public class SentimentResult
{
    public string Sentiment { get; set; }
}


// error example class
public class ErrorExample
{
    public string Category { get; set; }
    public string Incorrect { get; set; }
    public string Corrected { get; set; }
    public string Reasoning { get; set; }
}
