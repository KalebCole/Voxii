# Voxii - VR Language Learning App

An immersive virtual reality language learning windows desktop application. By simulating common real-world scenarios like ordering coffee in a café, Voxii allows users to learn and practice a new language through realistic, conversational interactions. View your score and error corrections any time during the conversation by a push of a button!

## Features

- **Life-like scenes**: Immerse yourself in different life-like scenes to 
- **Immersive real-life scenarios**: Practice in various common scenarios to hone your english speaking skills
- **Speech recognition and generation**: Speak directly into your microphone and Voxii will respond back, similar to real life
- **Avatar corrections**: If mistakes are made, the avatar politely corrects you
- **Score and corrections**: Get immediate feedback and improvement tips, along with your score for the conversation you've had so far, with the click of a button

## Getting started

1. Launch the game and hold the controller's primary button to start speaking
2. Listen to the response the AI gives, and continue conversing using step 1.
3. Press the controller's secondary button to get feedback on the conversation so far

## Documentation

Unity version - 2022.3.39f1

SDK version - .NET Framework

Note that only public methods are included here

### XRInputActions.inputactions
- Contains the mappings which map from the keyboard to the controller's controls

### AIVoice.cs
- Text to speech for the AI avatar

- `Speak(string msg)`
  - Asynchronously outputs AI speech for the given message using Piper TTS
 
- `SpeakRepeat()`
  - Asynchronously outputs the "Can you repeat that" message audio
 
- `SpeakInitialMsg()`
  - Asynchronously outputs the initial message for the specific avatar

 ### ChatLoop.cs
 - The user-avatar conversation loop

 - `SendUserMessage(string msg)`
   - Sends the given message to the AI avatar, adds it to the logs and chat history
