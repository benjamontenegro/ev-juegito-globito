using UnityEngine;
using UnityEngine.InputSystem;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip[] sfxClips;
    
    public void PlayAudio()
    {
        audioSource.Play();
    }
    public void PauseAudio()
    {
        audioSource.Pause();
    }

    public void PlaySoundRandom()
    {
        int randomnum = Random.Range(0, sfxClips.Length);
        audioSource.PlayOneShot(sfxClips[randomnum]);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            if (Keyboard.current.qKey.isPressed)
            {
                PlaySoundRandom();
            }
            else
            {
                if (!audioSource.isPlaying)
                    PlayAudio();
                else
                    PauseAudio();
            }
        }
    }
}
