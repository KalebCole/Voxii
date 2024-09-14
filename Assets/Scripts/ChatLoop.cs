# nullable enable

using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

public class ChatLoop : MonoBehaviour
{
    public GroqApiClient groqApi = new GroqApiClient();
    public bool isResponding = false;
    public string chatLogFilePath;

    private static readonly string initialAIMessage = "Hello, welcome to our cafe. What can I get for you today?";
    private static readonly OnboardingData onboardingData = new OnboardingData();
    private static readonly JObject systemPrompt = new JObject
    {
        ["role"] = "system",
        ["content"] = $@"
        As a proficient english speaking person,
    You are going to help another person learn the {onboardingData.LanguageToLearn} language
    
    You will do so by acting as the following:
    You are a {onboardingData.SceneToRole[onboardingData.Scene]} in a {onboardingData.Scene}.
    
    You are helping a customer named {onboardingData.PersonName}.
        This is their language proficiency in {onboardingData.LanguageToLearn}: {onboardingData.LanguageProficiency}
        They want to practice speaking these phrases: {string.Join(", ", onboardingData.PhrasesToWorkOn)}
    
    
    Output your responses in a friendly way that helps {onboardingData.PersonName} work on the phrases, keeping in mind their language proficiency of {onboardingData.LanguageProficiency}
    Do not act as a teacher or comment on the person's phrasing
    Do not overwhelm the person by asking too many questions
    Act as though this is a normal conversation
    When {onboardingData.PersonName} doesn't say have proper {onboardingData.LanguageToLearn}, act as if you were in a normal conversation and ask them a question while being confused
        "
    };
    private readonly JArray chatHistory = new JArray
    {
        systemPrompt,
        new JObject
        {
            ["role"] = "assistant",
            ["content"] = initialAIMessage
        }
    };

    private async void Start()
    {
        chatLogFilePath = Path.Combine(Application.persistentDataPath, "chat_log.txt");
        ClearChatLog();
        isResponding = true;
        await AIVoice.SpeakInitialMsg();
        isResponding = false;
    }

    private void ClearChatLog()
    {
        if (File.Exists(chatLogFilePath))
        {
            File.WriteAllText(chatLogFilePath, string.Empty);  // Clear the file
        }
        else
        {
            File.Create(chatLogFilePath).Dispose();  // Create the file if it doesn't exist
        }
    }

    private void LogPlayerMessage(string message)
    {
        File.AppendAllText(chatLogFilePath, message);
    }

    private void TrimChatHistory(int maxMessages = 5)
    {
        // +1 for the system prompt
        if (chatHistory.Count <= maxMessages * 2 + 1) return;
        chatHistory.RemoveAt(1);
        chatHistory.RemoveAt(1);
    }

    private string ParseAIMsg(string msg)
    {
        // remove anything within parentheses and parentheses itself
        msg = Regex.Replace(msg, @"\s*\(.*?\)\s*", "");

        // remove anything within asterisks and asterisks itself
        msg = Regex.Replace(msg, @"\s*\*.*?\*\s*", "");

        // get rid of any newlines too
        msg = msg.Replace("\n", ". ");

        msg = msg.Trim(' ', '"', '\'', '.');

        return msg;
    }

    public async Task SendUserMessage(string msg)
    {
        msg = msg.Trim(' ', '"', '\'');
        if (msg == "")
        {
            isResponding = true;
            await AIVoice.SpeakRepeat();
            isResponding = false;
            return;
        }

        Debug.Log("Sending message: " + msg);
        JObject userMessage = new JObject { ["role"] = "user", ["content"] = msg };

        LogPlayerMessage(msg);

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

        string contentStr = ParseAIMsg(content.ToString());

        JObject assistantMessage = new JObject
        {
            ["role"] = "assistant",
            ["content"] = contentStr
        };
        chatHistory.Add(assistantMessage);
        Debug.Log("Assistant: " + contentStr);

        await AIVoice.Speak(contentStr);
        isResponding = false;
    }
}
