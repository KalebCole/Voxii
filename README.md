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

## Demo Video
[![Watch the video](https://i.imgur.com/dn050iWl.png)](https://youtu.be/w4oMArZul2A)

**Click the image to watch the demo video**

## Documentation

Unity version - 2022.3.39f1

SDK version - .NET Framework

Note that only public methods are included here

### XRInputActions.inputactions
- Contains the mappings which map from the keyboard to the controller's controls

### AIVoice.cs (static class)
- Text to speech for the AI avatar

#### Methods

- `static async Task Speak(string msg)`
  - Asynchronously outputs AI speech for the given message using Piper TTS
 
- `static async Task SpeakRepeat()`
  - Outputs the "Can you repeat that" message audio
 
- `static async Task SpeakInitialMsg()`
  - Outputs the initial message for the specific avatar

### ChatLoop.cs
- The user-avatar conversation loop

#### Public variables:
- GroqApiClient groqApi
- bool isResponding: Keeps track of whether the AI is responding
- bool userSpeaking: Keeps track of whether the user is speaking
- string chatLogFilePath: The file path for the chat logs
- GameObject loadingSymbol
- int msgsSent
- TextMeshProUGUI messagesRemainingValue
- LevelManagement levelManagement

#### Methods

- `async Task SendUserMessage(string msg)`
  - Sends the given message to the AI avatar, adds it to the logs and chat history
- `void setIsResponding(bool value)`
  - setter for `isResponding` variable
- `void setUserSpeaking(bool value)`
  - setter for `userSpeaking` variable

### GameController.cs
- Handles game related logic

#### Components
- LoadingSymbolController loadingSymbolController
- Button testButton

#### Methods

- `void StartLoading()`
  - Initializes the loading icon logic
 
### LoadingSymbolController.cs
- Handles the loading symbol logic

#### Components
- GameObject loadingSymbol

#### Methods

- `void ShowLoadingSymbol()`
  - Shows the loading symbol
 
- `void HideLoadingSymbol()`
  - Hides the loading symbol

### GroqApiClient.cs
- Handles the requests sent to Groq's llama3

#### Methods

- `GroqApiClient(string apiKey = "")`
  - Initializes the groq api client with the API key
  - If no API key is provided it will try searching for one in a `.env` file

- `async Task<JObject?> CreateChatCompletionAsync(JObject request)`
  - The request to send to the AI, including the parameters and the chat history

### MicRecorder.cs
- Handles recording through the microphone

#### Components
- GameObject loadingSymbol

#### Methods

- `void StartRecording()`
  - Starts recording the microphone

- `void StopRecording()`
  - Stops recording the microphone
 
- `void SaveRecording()`
  - Saves the recording in the persistent data path as "recording.wav"
 
- `void PlayRecording()`
  - Plays the recording
 
### OnboardingData.cs
- Data container class to hold onboarding data instances

#### Public variables
- string PersonName { get; set; }
- string LanguageProficiency { get; set; }
- string LanguageToLearn { get; set; }
- List<string> PhrasesToWorkOn { get; set; }
- string Scene { get; set; }
- Dictionary<string, string> SceneToRole { get; set; }

### PrimaryBtnHold.cs
- Logic for what should happen when the primary button is held

#### Components
- InputActionAsset inputActionAsset
- MicRecorder micRecorder
- WhisperTranscriber whisperTranscriber
- ChatLoop chatLoop

#### Public variables
- bool isRecording: Tracks whether the application is recording

### SecondaryBtnPress.cs
- Logic for what should happen when the secondary button is pressed

#### Components
- InputActionAsset inputActionAsset
- ChatLoop chatLoop
- PrimaryBtnHold primaryBtnHold

### SaveWavFile.cs (static class - WavUtility)
- Contains logic to save an audio clip to a location in the `.wav` format

#### Methods

- `static byte[] FromAudioClip(AudioClip clip)`
  - Converts an audio clip into byte[]
 
- `static void SaveWav(string filePath, AudioClip clip)`
  - Saves the audio clip as a `.wav` file into the specified path
 
### Scorer.cs
- Contains the AI scoring logic for the user's english

#### Public variables
- GroqApiClient groqApi

#### Methods

- `Scorer(string chatLogFilePath)`
  - Initializes the scorer with the chat logs of the conversation so far
 
- `async Task<string> GetScore()`
  - Gets the score on the user's english of the conversation
 
### WhisperTranscriber.cs
- Contains the logic for transcribing the user's speech to text

#### Components
- TextMeshProUGUI displayText

#### Methods

- `async Task<string> TranscribeRecording()`
  - Gets the string for the user's speech stored in `recording.wav` in the application's persistent data path
 

### BackgroundMusicController.cs

- Controls the background music

#### Public variables
- Button muteButton
- GameObject symbol: symbol to show the mute button
- Sprite unmutedImage
- Sprite mutedImage

#### Methods
- `void ToggleMute`
- `UpdateButtonImage`
  - Changes the button image based on the mute state

### FeedbackScreenController.cs
- Controls the feedback screen

#### Public variables
- GameObject screen1;
- GameObject screen2;
- Button nextButton;
- Button doneButton;
- TextMeshProUGUI pointValue;
- TextMeshProUGUI grammarErrorValue;
- TextMeshProUGUI responseTimeValue;
- TextMeshProUGUI relevanceValue;
- TextMeshProUGUI feedbackCategory;
- TextMeshProUGUI feedbackIncorrect;
- TextMeshProUGUI feedbackCorrected;
- TextMeshProUGUI feedbackReasoning;

#### Methods

- `void ShowSecondScreen`
- `void ChangeToMenuScene`

### LevelManagement.cs
- Manages the levels

#### Public variables
- GameObject display1
- GameObject display2

#### Methods
- `void goToMainMenu`
- `void goToPostLevel`
- `void switchDisplays`
