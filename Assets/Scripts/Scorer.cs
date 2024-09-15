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

    private JObject systemPrompt // this dynamically generates upon instantiation of the scorer class (so we can interpolate the chat log file)
    {
        get
        {
            return new JObject
            {
                ["role"] = "system",
                ["content"] = $@"
                  You are proficient in the English Language 


                        I want you to grade the user in a conversation between a user and an assistant based on the following criteria: 
                        1. Number of errors in the user's grammar
                        2. Average time for the response in seconds
                        3. Analysis of the relevance of the user response to gauge if they understood what the assistant was asking

                        Here is the conversation:
                        

                "
            };
        }
    }

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
                        Number of errors: number,
                        Average time for response: number in seconds,
                        Accuracy of understanding and responding: percentage given evaluation and semantics
";
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
                ["temperature"] = 1.2
            };

            UnityEngine.Debug.Log("Request: " + request);
            JObject? response = await groqApi.CreateChatCompletionAsync(request);
            UnityEngine.Debug.Log("after request sent");

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





// Request: {
//   "model": "llama-3.1-8b-instant",
//   "messages": [
//     {
//       "role": "system",
//       "content": "\r\n                  You are proficient in the English Language \r\n\r\n\r\n                        I want you to grade the user in a conversation between a user and an assistant based on the following criteria: \r\n                        1. Number of errors in the user's grammar\r\n                        2. Average time for the response in seconds\r\n                        3. Analysis of the relevance of the user response to gauge if they understood what the assistant was asking\r\n\r\n                        Here is the conversation:\r\n\r\n                "
//     },
//     "user: Thank you very much for having up here.\nassistant: Hello! No problem at all. I have a comfortable chair for you. It was nice meeting you, how are you today? Can I ask, would you like a coffee or... something else to start?"
//   ],
//   "max_tokens": 100,
//   "temperature": 1.2
// }
// UnityEngine.Debug:Log (object)
// Scorer/<GetScore>d__6:MoveNext () (at Assets/Scripts/Scorer.cs:79)
// System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<string>:Start<Scorer/<GetScore>d__6> (Scorer/<GetScore>d__6&)
// Scorer:GetScore ()
// SecondaryBtnPress/<HandlePressAsync>d__7:MoveNext () (at Assets/Scripts/SecondaryBtnPress.cs:60)
// System.Runtime.CompilerServices.AsyncTaskMethodBuilder:Start<SecondaryBtnPress/<HandlePressAsync>d__7> (SecondaryBtnPress/<HandlePressAsync>d__7&)
// SecondaryBtnPress:HandlePressAsync ()
// SecondaryBtnPress:OnPress (UnityEngine.InputSystem.InputAction/CallbackContext) (at Assets/Scripts/SecondaryBtnPress.cs:36)
// UnityEngine.InputSystem.LowLevel.NativeInputRuntime/<>c__DisplayClass7_0:<set_onUpdate>b__0 (UnityEngineInternal.Input.NativeInputUpdateType,UnityEngineInternal.Input.NativeInputEventBuffer*)
// UnityEngineInternal.Input.NativeInputSystem:NotifyUpdate (UnityEngineInternal.Input.NativeInputUpdateType,intptr)
