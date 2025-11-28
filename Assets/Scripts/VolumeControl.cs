using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    public AudioMixerGroup master, music, sfx;
    public Slider masterSlider, musicSlider, sfxSlider;

    private string masterVolume = "VolumeMaster";
    private string musicVolume = "VolumeMusic";
    private string sfxVolume = "VolumeSFX";

    void Start()
    {
        float _masterVolume = 1;
        float _sfxVolume = 1;
        float _musicVolume = 1;

        master.audioMixer.GetFloat(masterVolume, out _masterVolume);
        music.audioMixer.GetFloat(musicVolume, out _musicVolume);
        sfx.audioMixer.GetFloat(sfxVolume, out _sfxVolume);

        masterSlider.value = _masterVolume;
        musicSlider.value = _musicVolume;
        sfxSlider.value = _sfxVolume;
    }

    public void SetMasterVolume(float volume)
    {
        master.audioMixer.SetFloat(masterVolume, volume);
    }

    public void SetMusicVolume(float volume)
    {
        music.audioMixer.SetFloat(musicVolume, volume);
    }

    public void SetSfxVolume(float volume)
    {
        sfx.audioMixer.SetFloat(sfxVolume, volume);
    }
}
