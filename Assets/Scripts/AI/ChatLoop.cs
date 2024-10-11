# nullable enable

using UnityEngine;
using UnityEngine.XR.OpenXR;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using TMPro;
using System.Collections.Generic;

public class ChatLoop : MonoBehaviour
{
    public Animator animator;
    public GroqApiClient groqApi = new GroqApiClient();
    public bool isResponding = false;
    public bool userSpeaking = false;
    public string chatLogFilePath;
    public GameObject loadingSymbol;
    public int msgsSent { get; private set; } = 0;

    private int maxMessages = 4;
    public TextMeshProUGUI messagesRemainingValue;
    public LevelManagement levelManagement;

    public PrimaryBtnHold primaryBtnHold;

    private static readonly string initialAIMessage = "Hello, welcome to our cafe. What can I get for you today?";
    //private static readonly OnboardingData onboardingData = new OnboardingData();
    private static readonly JObject systemPrompt = new JObject
    {
        ["role"] = "system",
        ["content"] = $@"
         You are in a {MenuData.SceneSelection}, acting as a Male {MenuData.getRole()}. Your goal is to create a comfortable, immersive environment for the user to practice english.

        Your conversation partner, has a language proficiency level of {MenuData.LanguageProficiency} out of 5 in english and wishes to practice the following phrases: {string.Join(", ", MenuData.OptionsSelected)}.

        You should have hostility level {MenuData.AvatarHostility} out of 5, where at 1 you are incredibly patient and kind, and at 5 you are impatient and less forgiving to the user.
        
        Respond naturally, staying fully in character as a {MenuData.getRole()}, and keep the conversation flowing while adapting to the user's proficiency level.
        Stay authentic to your role, ensuring the conversation feels real and immersive.
        Focus on helping the user build fluency by guiding the conversation naturally.
        Do not ask more than 2 questions per response.
        Do not explicitly correct or act as a teacher.
        Keep the interaction conversational, avoiding excessive questions or over-complication.

        When the user uses an incorrect phrase or structure, repeat what they said but in the correct format, and do so in a natural, friendly manner that encourages learning without direct correction.
        Here is an example of a correct response when the user uses an incorrect phrase or structure the user uses an incorrect phrase or structure:

        User: “Me want big coffee.”
        Assistant: “Haha, did you mean to say that you want to order a large coffee?”
        This helps the user understand the correct phrasing without feeling like they are being explicitly corrected, maintaining a light and engaging conversation.

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

    public void setIsResponding(bool value)
    {
        isResponding = value;
        animator.SetBool("isResponding", value);
    }

    public void setUserSpeaking(bool value)
    {
        userSpeaking = value;
        animator.SetBool("userSpeaking", value);
    }

    private async void Start()
    {
        messagesRemainingValue.text = maxMessages.ToString();

        Time.timeScale = 0;

        // Delay for 1 second to allow the VR headset to load
        await Task.Delay(1000);

        Time.timeScale = 1;

        Debug.Log("Avatar Hostility:" + MenuData.AvatarHostility);
        chatLogFilePath = Path.Combine(Application.persistentDataPath, "chat_log.txt");
        ClearChatLog();
        setIsResponding(true);
        await AIVoice.SpeakInitialMsg();
        setIsResponding(false);
    }

    private void Update()
    {

        animator.SetInteger("speakingIdx", Random.Range(0, 3));
        animator.SetInteger("emotionIdx", Random.Range(0, 1));
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
        // Get current time
        var time = System.DateTime.Now.ToString("HH:mm:ss");
        // Log with time
        string logEntry = $"{time} {role}: {message}\n";
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
            setIsResponding(true);
            await AIVoice.SpeakRepeat();
            setIsResponding(false);
            return;
        }
        // Update the number of messages remaining
        ++msgsSent;
        int temp = maxMessages - msgsSent;
        messagesRemainingValue.text = temp.ToString();

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
            ["temperature"] = 0.4
        };

        SetLoadingSymbolVisibility(true);


        JObject? response = await groqApi.CreateChatCompletionAsync(request);
        var content = response?["choices"]?[0]?["message"]?["content"];

        SetLoadingSymbolVisibility(false);

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

        // debug
        Debug.Log("Before GetSentiment");

        // we need to await the assistant speaking before we can get the sentiment
        bool sentiment = await GetSentimentAsync(contentStr);

        // debug
        Debug.Log("After GetSentiment");

        animator.SetBool("happy", sentiment);

        setIsResponding(true);
        await AIVoice.Speak(contentStr);
        setIsResponding(false);

        // When max number of messages have been set, switch to 'finished level' UI
        if (msgsSent == maxMessages)
        {
            primaryBtnHold.enabled = false;
            await ScoreLevelAsync();
            levelManagement.switchDisplays();
        }

    }

    private async Task<bool> GetSentimentAsync(string msg)
    {
        SentimentAnalyzer sentimentAnalyzer = new SentimentAnalyzer();
        return await sentimentAnalyzer.IsPositiveOrNeutralSentiment(msg);
        // return true;
    }

    private void SetLoadingSymbolVisibility(bool isVisible)
    {
        if (loadingSymbol != null)
        {
            loadingSymbol.SetActive(isVisible);
        }
    }

    public async Task ScoreLevelAsync()
    {
        // Create the Scorer with mock mode based on the useMockData flag
        Scorer scorer = new Scorer(chatLogFilePath);

        // Get the points from the scorer
        int points = await scorer.CalculatePointsAsync();
        (ScoreResult, float, List<ErrorExample>) values = await scorer.GetResultsAndResponseTimeAsync();

        // debug values by logging the list of error examples
        foreach (ErrorExample error in values.Item3)
        {
            Debug.Log($"Error in  score leel: {error}");
        }


        Debug.Log($"Points in Scdareybutn pres: {points}");



        // Save to static data class to be access by results UI
        ResultsData.points = points;

        // if errors is 0, item3 will be empty so set the feedback to empty strings
        ResultsData.errors = values.Item1.NumberOfErrors;
        ResultsData.relevanceScore = values.Item1.Accuracy * 10;
        ResultsData.responseTime = (int)values.Item2;
        if (values.Item3.Count > 0) // no errors
        {
            // debug
            Debug.Log("Errors in score level");
            ResultsData.feedbackCategory = values.Item3[0].Category;
            ResultsData.feedbackIncorrect = values.Item3[0].Incorrect;
            ResultsData.feedbackCorrected = values.Item3[0].Corrected;
            ResultsData.feedbackReasoning = values.Item3[0].Reasoning;
        }
        else
        {
            // debug
            Debug.Log("No errors in score level");
            ResultsData.feedbackCategory = "";
            ResultsData.feedbackIncorrect = "";
            ResultsData.feedbackCorrected = "";
            ResultsData.feedbackReasoning = "";
        }

    }
}
