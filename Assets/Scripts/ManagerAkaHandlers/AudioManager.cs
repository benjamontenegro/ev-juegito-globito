using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource AudioSrc;
    [SerializeField] private List<SfxClip> AllSounds;

    public struct SfxClip
    {
        public AudioClip clip;
        public string sfxName;

        public SfxClip (string name, string resourcePath)
        {
            clip = LoadAudioClip(resourcePath);
            sfxName = name;
        }
    }

    void Awake()
    {
        AllSounds = new List<SfxClip>();
        AudioSrc = GetComponent<AudioSource>();
        LoadAllSfxClips();
    }

    #region FUNCIONES
    
    /// <summary> reproducir audio segun nombre (anotados aca) </summary>
    public void PlayAudioClip(string clipName)
    {
        AudioClip clip = AllSounds.Find(x => x.sfxName == clipName).clip;
        if (clip != null)
            AudioSrc.PlayOneShot(clip);
    }

    /// <summary> PlayAudioClip: lo mismo en 3d </summary>
    public void PlayAudioClipAtPlace(string clipName, Vector2 position)
    {
        // ESTOE S TEMPORAL,
        // HACER UIN AUDIOSORUCE POOL EN EL FUUTURO
        AudioClip clip = AllSounds.Find(x => x.sfxName == clipName).clip;
        
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, position);
    }

    private void LoadAllSfxClips()
    {
        AllSounds.Add(new SfxClip("jump", "Audio/kirbyjump1"));
        AllSounds.Add(new SfxClip("land", "Audio/kirbyland1"));
        AllSounds.Add(new SfxClip("hurt", "Audio/kirbyhurt"));
        AllSounds.Add(new SfxClip("bump", "Audio/sfxbump"));
    }

    public static AudioClip LoadAudioClip(string audioPath)
    {
        AudioClip sfx = Resources.Load<AudioClip>(audioPath);

        // if (sfx != null)
        //     Debug.Log($"SOUND LOADED BRO (from {audioPath})");
        // else
        //     Debug.Log($"could load {audioPath}");
        // PENDIENTE
        // VER COMO HACER QUE CARGUE UNA VEZ 
        // Y NO CADA VEZ QUE CARGUE LA ESCENA

        return sfx;
    }

    #endregion
}
