using UnityEditor.Timeline.Actions;
using UnityEngine;
// using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    private AudioSource AudioSrc;
    public AudioClip JumpSound;
    public AudioClip LandSound;
    
    public void PlayAudioClip(AudioClip clip)
    {
        AudioSrc.PlayOneShot(clip);
    }

    private void Awake()
    {
        AudioSrc = GetComponent<AudioSource>();
        JumpSound = LoadAudioClip("Audio/kirbyjump1"); 
        LandSound = LoadAudioClip("Audio/kirbyland1");
    }

    #region EXTRA SHIT

    private AudioClip LoadAudioClip(string audioPath)
    {
        AudioClip sfx = Resources.Load<AudioClip>(audioPath);

        if (sfx != null)
            Debug.Log($"SOUND LOADED BRO (from {audioPath})");
        else
            Debug.Log($"could load {audioPath}");
        // PENDIENTE
        // VER COMO HACER QUE CARGUE UNA VEZ 
        // Y NO CADA VEZ QUE CARGUE LA ESCENA

        return sfx;
    }

    #endregion
}
