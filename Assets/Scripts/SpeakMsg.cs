using UnityEngine;

public static class SpeakMsg
{
    public static void SpeakMsg(string msg)
    {
        GameObject aiVoiceObject = GameObject.Find("AIVoice");

        if (aiVoiceObject == null)
        {
            Debug.LogError("GameObject 'AIVoice' not found in the scene.");
            return;
        }

        AudioSource audioSource = aiVoiceObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = aiVoiceObject.AddComponent<AudioSource>();
        }

        audioSource.clip = Resources.Load<AudioClip>("Audio/" + msg);

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Audio clip not found: " + msg);
        }
    }
}
