using UnityEngine;
using UnityEngine.Audio;// librería sistema de audio Unity
using DG.Tweening; // TWEENS

public class SoundList : MonoBehaviour
{
    public Sound[] soundList;
    private float minPitch = 0.85f;
    private float maxPitch = 1.25f;

    void Start()
    {
        for (int i = 0; i < soundList.Length; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            soundList[i].SetAudioSource(audioSource);
        }
    }

    public void PlaySound(string audioName)
    {
        Sound sound = FindSound(audioName);
        sound.source.Play();
    }

    public void PlaySoundRandomPitch(string audioName)
    {
        Sound sound = FindSound(audioName);
        sound.source.pitch = Random.Range(minPitch, maxPitch);
        sound.source.Play();
    }

    public void SoundFadeIn(string audioName, float fadeTime, float delay)
    {
        Sound sound = FindSound(audioName);
        sound.source.volume = 0;
        sound.source.Play();
        sound.source.DOFade(1, fadeTime).SetUpdate(true).SetDelay(delay);
    }

    public void StopSound(string audioName)
    {
        Sound sound = FindSound(audioName);
        sound.source.Stop();
    }

    public Sound FindSound(string soundName)
    {
        for (int i = 0; i < soundList.Length; i++)
        {
            if (soundName == soundList[i].audioName)
            {
                return soundList[i];
            }
        }

        return null;
    }

}

[System.Serializable]
public class Sound
{
    public string audioName;
    public AudioClip clip;
    public float volume;
    public float pitch;
    public bool loop;

    public AudioMixerGroup mixer;

    [HideInInspector]
    public AudioSource source;

    public void SetAudioSource(AudioSource audioSource)
    {
        source = audioSource;
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.outputAudioMixerGroup = mixer;
        source.loop = loop;
        source.playOnAwake = false;
    }
}




