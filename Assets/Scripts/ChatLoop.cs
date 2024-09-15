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
         You are in a {onboardingData.Scene}, acting as a {onboardingData.SceneToRole[onboardingData.Scene]}. Your goal is to create a comfortable, immersive environment for {onboardingData.PersonName} to practice {onboardingData.LanguageToLearn}.

        Your conversation partner, {onboardingData.PersonName}, has a {onboardingData.LanguageProficiency} level in {onboardingData.LanguageToLearn} and wishes to practice the following phrases: {string.Join(", ", onboardingData.PhrasesToWorkOn)}.

        
        Respond naturally, staying fully in character as a {onboardingData.SceneToRole[onboardingData.Scene]}, and keep the conversation flowing while adapting to {onboardingData.PersonName}'s proficiency level.
        Stay authentic to your role, ensuring the conversation feels real and immersive.
        Focus on helping {onboardingData.PersonName} build fluency by guiding the conversation naturally.
        Do not ask more than 2 questions per response.
        Do not explicitly correct or act as a teacher.
        Keep the interaction conversational, avoiding excessive questions or over-complication.

        When {onboardingData.PersonName} uses an incorrect phrase or structure, repeat what they said but in the correct format, and do so in a natural, friendly manner that encourages learning without direct correction.
        Here is an example of a correct response when {onboardingData.PersonName} uses an incorrect phrase or structure {onboardingData.PersonName} uses an incorrect phrase or structure:

        User: “Me want big coffee.”
        Assistant: “Haha, did you mean to say that you want to order a large coffee?”
        This helps {onboardingData.PersonName} understand the correct phrasing without feeling like they are being explicitly corrected, maintaining a light and engaging conversation.

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

    private void LogMessage(string role, string message)
    {
        /// Ensure the message is trimmed and not empty
        message = message.Trim();
        if (string.IsNullOrEmpty(message))
        {
            Debug.LogWarning("Attempted to log an empty message.");
            return;
        }
        string logEntry = $"{role}: {message}\n";
        File.AppendAllText(chatLogFilePath, logEntry);
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

        LogMessage("user", msg); ;

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
        LogMessage("assistant", contentStr);
        Debug.Log("Assistant: " + contentStr);

        await AIVoice.Speak(contentStr);
        isResponding = false;
    }
}
