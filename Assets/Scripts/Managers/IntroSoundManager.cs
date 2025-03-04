using UnityEngine;

public class IntroSoundManager : MonoBehaviour
{
    public AudioClip[] dispatchSounds;

    private short dispatchIndex = 0;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetNullAudioMixerGroup()
    {
        audioSource.outputAudioMixerGroup = null;
    }

    public void PlayDispatchSound()
    {
        audioSource.clip = dispatchSounds[dispatchIndex];
        audioSource.Play();
        dispatchIndex++;
    }
}
