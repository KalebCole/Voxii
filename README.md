# Voxii - VR Language Learning App

An immersive virtual reality language learning windows desktop application. By simulating common real-world scenarios like ordering coffee in a café, Voxii allows users to learn and practice a new language through realistic, conversational interactions. View your score and error corrections any time during the conversation by a push of a button!

## Features

- **Life-like scenes**: Immerse yourself in a different life-like scene to practice speaking
- **Immersive real-life scenarios**: Practice in a common scenario to hone your english speaking skills
- **Speech recognition and generation**: Speak directly into your microphone and the Voxii avatar will respond back, similar to real life
- **Avatar corrections**: If mistakes are made, the avatar politely corrects you
- **Score and corrections**: Get immediate feedback and improvement tips, along with your score for the conversation you've had so far, with the click of a button

## Getting started

1. Create an API key from [Groq](https://groq.com/) and paste it in a `.env` file at the root location, in the format: `GROQ_API_KEY=<YOUR_GROQ_API_KEY_HERE>`
2. Launch the game and hold the controller's primary button to start speaking
3. Listen to the response the AI gives, and continue conversing using step 1.
4. Press the controller's secondary button to get feedback on the conversation so far

## Demo

|  | Demos |
|-------|------------|
|  
**👉 Click the image to watch the demo video** |  [![Watch the video](https://i.imgur.com/7LKoxNl.png)](https://drive.google.com/file/d/1-dYw1LEjvwBioJJgKyHAPzA2zJ9YAJmm/view?usp=sharing) 
**👉 Click the image to view the pitch deck** |[![View the pitch deck](https://i.imgur.com/MciAf0O.png)](https://docs.google.com/presentation/d/e/2PACX-1vQlnCTSOKKt2h08IDA2N8Pc5B1LLoYhsxkzwXXdgnDHhHX_r1eM3tADeDBCC66Xyw/pub?start=false&loop=false&delayms=3000) 

---


# Documentation

![Unity](https://img.shields.io/badge/Unity-2022.3.39f1-blue) ![SDK](https://img.shields.io/badge/SDK-.NET_Framework-blue)

*Note: Only public methods are included here.*

## 📑 Table of Contents

1. [Folder Structure](#folder-structure-within-assetscripts)
2. [Classes in Each Folder](#classes-in-each-folder)
   - [AI 🤖](#assetsai)
     - [AIVoice.cs 🎤](#aivoicecs-static-class)
     - [ChatLoop.cs 🔄](#chatloopcs)
     - [GroqApiClient.cs 📡](#groqapiclientcs)
     - [Scorer.cs 📝](#scorercs)
     - [WhisperTranscriber.cs 🗣️](#whispertranscribercs)
     - [SentimentAnalyzer.cs 😊](#sentimentanalyzercs)
   - [LevelManagement 🎮](#assetslevelmanagement)
     - [OnboardingData.cs 📄](#onboardingdatacs)
     - [FeedbackScreenController.cs 💬](#feedbackscreencontrollercs)
     - [LevelManagement.cs ↔️](#levelmanagementcs)
     - [MenuData.cs 🗂️](#menudatacs-static-class)
     - [ResultsData 📊](#resultsdata)
   - [Utility 🛠️](#assetsutility)
     - [XRInputActions.inputactions 🎮](#xrinputactionsinputactions)
     - [LoadingSymbolController.cs ⏳](#loadingsymbolcontrollercs)
     - [MicRecorder.cs 🎙️](#micrecordercs)
     - [PrimaryBtnHold.cs ▶️](#primarybtnholdcs)
     - [SecondaryBtnPress.cs 🔁](#secondarybtnpresscs)
     - [SaveWavFile.cs (WavUtility) 💾](#savewavfilecs-static-class---wavutility)
     - [BackgroundMusicController.cs 🎵](#backgroundmusiccontrollercs)

---

## 📂 Folder Structure within `/Assets/Scripts`

- **AI** 🤖: Contains the AI avatar's logic and the Grammar Checker AI logic
- **LevelManagement** 🎮: Contains the logic for managing the various scenes from the main menu to the post-level screen
- **Utility** 🛠️: Contains utility scripts like the `SendWavFile` for saving audio clips as `.wav` files

## 📚 Classes in Each Folder

### /Assets/AI

<details>
  <summary><strong>AIVoice.cs</strong> (static class) 🎤</summary>

  **Description:**  
  Text to speech for the AI avatar

  ##### 🛠️ Methods
  - `static async Task Speak(string msg)`
    - Asynchronously outputs AI speech for the given message using Piper TTS
  - `static async Task SpeakRepeat()`
    - Outputs the "Can you repeat that" message audio
  - `static async Task SpeakInitialMsg()`
    - Outputs the initial message for the specific avatar

</details>

<details>
  <summary><strong>ChatLoop.cs</strong> 🔄</summary>

  **Description:**  
  The user-avatar conversation loop

  ##### ⚙️ Public Variables
  - `GroqApiClient groqApi`
  - `bool isResponding`: Tracks if the AI is responding
  - `bool userSpeaking`: Tracks if the user is speaking
  - `string chatLogFilePath`: File path for chat logs
  - `GameObject loadingSymbol`
  - `int msgsSent`
  - `TextMeshProUGUI messagesRemainingValue`
  - `LevelManagement levelManagement`

  ##### 🛠️ Methods
  - `async Task SendUserMessage(string msg)`
    - Sends a message to the AI avatar, adds it to logs and chat history
  - `void setIsResponding(bool value)`
    - Setter for `isResponding`
  - `void setUserSpeaking(bool value)`
    - Setter for `userSpeaking`

</details>

<details>
  <summary><strong>GroqApiClient.cs</strong> 📡</summary>

  **Description:**  
  Handles requests sent to Groq's llama3

  ##### 🛠️ Methods
  - `GroqApiClient(string apiKey = "")`
    - Initializes the Groq API client with an API key or from a `.env` file
  - `async Task<JObject?> CreateChatCompletionAsync(JObject request)`
    - Sends a request to the AI, including parameters and chat history

</details>

<details>
  <summary><strong>Scorer.cs</strong> 📝</summary>

  **Description:**  
  Contains the AI scoring logic for the user's English

  ##### ⚙️ Public Variables
  - `GroqApiClient groqApi`

  ##### 🛠️ Methods
  - `Scorer(string chatLogFilePath)`
    - Initializes the scorer with the chat logs of the conversation so far
  - `async Task<string> GetScore()`
    - Gets the score on the user's English of the conversation
  - `async Task<string> GetResponseOutput(JArray msgs)`
    - Overload the GetResponseOutput method to accept a JArray parameter
    - Useful for testing the scoring system with different chat logs
  - `ScoreResult ParseScoreNumbers(string scoreString)`
    - Parses the score string to extract the number of errors and accuracy
  - `SentimentResult ParseSentiment(string scoreString)`
    - Parses the sentiment from the AI response
  - `public List<ErrorExample> ParseErrorExamples(string scoreString)`
    - Parses the error examples from the AI response
  - `float CalculateResponseTime(string chatLogFilePath)`
    - Calculates the time taken for the user to respond to the AI
  - `static int CalculatePoints(ScoreResult scoreResult, float responseTime)`
    - Calculates the points based on the score result
  - `async Task<int> CalculatePointsAsync()`
    - Step-by-step calculation of points
  - `async Task<(ScoreResult, float, List<ErrorExample>)> GetResultsAndResponseTimeAsync`
    - Gets ScoreResult object
  - `class ScoreResult`
    - Holds data needed for score result (NumberOfErrors, Accuracy)
  - `class SentimentResult`
    - Holds data needed for sentiment result (Sentiment)
  - `class ErrorExample`
    - Holds data needed for error examples (Category, Incorrect, Corrected, Reasoning)

</details>

<details>
  <summary><strong>WhisperTranscriber.cs</strong> 🗣️</summary>

  **Description:**  
  Contains the logic for transcribing the user's speech to text

  ##### 🛠️ Components
  - `TextMeshProUGUI displayText`

  ##### 🛠️ Methods
  - `async Task<string> TranscribeRecording()`
    - Retrieves the string for the user's speech stored in `recording.wav` in the application's persistent data path

</details>

<details>
  <summary><strong>SentimentAnalyzer.cs</strong> 😊</summary>

  **Description:**  
  Analyzes the sentiment of the AI response each time the AI responds  
  Used to update the animation of the AI avatar

  ##### 🛠️ Methods
  - `public async Task<bool> IsPositiveOrNeutralSentiment(string message)`
    - Sends the AI avatar's response to the Groq API to analyze the sentiment
    - Returns `true` if the sentiment is positive or neutral, `false` otherwise

</details>

### /Assets/LevelManagement

<details>
  <summary><strong>OnboardingData.cs</strong> 📄</summary>

  **Description:**  
  Data container class to hold onboarding data instances

  ##### ⚙️ Public Variables
  - `string PersonName { get; set; }`
  - `string LanguageProficiency { get; set; }`
  - `string LanguageToLearn { get; set; }`
  - `List<string> PhrasesToWorkOn { get; set; }`
  - `string Scene { get; set; }`
  - `Dictionary<string, string> SceneToRole { get; set; }`

</details>

<details>
  <summary><strong>FeedbackScreenController.cs</strong> 💬</summary>

  **Description:**  
  Controls the feedback screen

  ##### ⚙️ Public Variables
  - `GameObject screen1`
  - `GameObject screen2`
  - `Button nextButton`
  - `Button doneButton`
  - `TextMeshProUGUI pointValue`
  - `TextMeshProUGUI grammarErrorValue`
  - `TextMeshProUGUI responseTimeValue`
  - `TextMeshProUGUI relevanceValue`
  - `TextMeshProUGUI feedbackCategory`
  - `TextMeshProUGUI feedbackIncorrect`
  - `TextMeshProUGUI feedbackCorrected`
  - `TextMeshProUGUI feedbackReasoning`

  ##### 🛠️ Methods
  - `void ShowSecondScreen()`
  - `void ChangeToMenuScene()`

</details>

<details>
  <summary><strong>LevelManagement.cs</strong> ↔️</summary>

  **Description:**  
  Switches between the main menu and the post-level screen displays based on the scene

  ##### ⚙️ Public Variables
  - `GameObject display1`
  - `GameObject display2`

  ##### 🛠️ Methods
  - `void goToMainMenu()`
  - `void goToPostLevel()`
  - `void switchDisplays()`

</details>

<details>
  <summary><strong>MenuData.cs</strong> 🗂️ (static class - WavUtility)</summary>

  **Description:**  
  Handles menu data storage and retrieval

  ##### ⚙️ Public Variables
  - `static List<bool> OptionsSelected`
  - `static string SceneSelection`
  - `static float LanguageProficiency`
  - `static float AvatarHostility`
  - `static string filePath`

  ##### 🛠️ Methods
  - `static void SetFilePath(string appDataPath)`
    - Needed because `Application.persistentDataPath` can't be accessed by a static non-MonoBehavior class
  - `static void SaveDataToJson()`
  - `static void LoadDataFromJson()`
  - `static string getRole()`
    - Retrieves the role based on the scene
  - `class MenuDataModel`
    - Stores necessary data (`OptionsSelected`, `SceneSelection`, `LanguageProficiency`, `AvatarHostility`)

</details>

<details>
  <summary><strong>ResultsData.cs</strong> 📊</summary>

  **Description:**  
  Class to store result data

  ##### ⚙️ Public Variables
  - `static int points`
  - `static int errors`
  - `static int relevanceScore`
  - `static int responseTime`
  - `static string feedbackCategory`
  - `static string feedbackIncorrect`
  - `static string feedbackCorrected`
  - `static string feedbackReasoning`

</details>

### /Assets/Utility

<details>
  <summary><strong>XRInputActions.inputactions</strong> 🎮</summary>

  **Description:**  
  Contains the mappings which map from the keyboard to the controller's controls

  ##### 🛠️ Methods
  - `void StartLoading()`
    - Initializes the loading icon logic

</details>

<details>
  <summary><strong>LoadingSymbolController.cs</strong> ⏳</summary>

  **Description:**  
  Handles the loading symbol logic

  ##### 🛠️ Components
  - `GameObject loadingSymbol`

  ##### 🛠️ Methods
  - `void ShowLoadingSymbol()`
    - Displays the loading symbol
  - `void HideLoadingSymbol()`
    - Hides the loading symbol

</details>

<details>
  <summary><strong>MicRecorder.cs</strong> 🎙️</summary>

  **Description:**  
  Handles recording through the microphone

  ##### 🛠️ Components
  - `GameObject loadingSymbol`

  ##### 🛠️ Methods
  - `void StartRecording()`
    - Starts recording the microphone
  - `void StopRecording()`
    - Stops recording the microphone
  - `void SaveRecording()`
    - Saves the recording in the persistent data path as `recording.wav`
  - `void PlayRecording()`
    - Plays the recording

</details>

<details>
  <summary><strong>PrimaryBtnHold.cs</strong> ▶️</summary>

  **Description:**  
  Logic for actions when the primary button is held

  ##### 🛠️ Components
  - `InputActionAsset inputActionAsset`
  - `MicRecorder micRecorder`
  - `WhisperTranscriber whisperTranscriber`
  - `ChatLoop chatLoop`

  ##### ⚙️ Public Variables
  - `bool isRecording`: Tracks whether the application is recording

</details>

<details>
  <summary><strong>SecondaryBtnPress.cs</strong> 🔁</summary>

  **Description:**  
  Logic for actions when the secondary button is pressed

  ##### 🛠️ Components
  - `InputActionAsset inputActionAsset`
  - `ChatLoop chatLoop`
  - `PrimaryBtnHold primaryBtnHold`

</details>

<details>
  <summary><strong>SaveWavFile.cs</strong> 💾 (static class - WavUtility)</summary>

  **Description:**  
  Contains logic to save an audio clip in `.wav` format

  ##### 🛠️ Methods
  - `static byte[] FromAudioClip(AudioClip clip)`
    - Converts an audio clip into `byte[]`
  - `static void SaveWav(string filePath, AudioClip clip)`
    - Saves the audio clip as a `.wav` file to the specified path

</details>

<details>
  <summary><strong>BackgroundMusicController.cs</strong> 🎵</summary>

  **Description:**  
  Controls the background music

  ##### ⚙️ Public Variables
  - `Button muteButton`
  - `GameObject symbol`: Symbol to show the mute button
  - `Sprite unmutedImage`
  - `Sprite mutedImage`

  ##### 🛠️ Methods
  - `void ToggleMute()`
  - `void UpdateButtonImage()`
    - Changes the button image based on the mute state

</details>

---