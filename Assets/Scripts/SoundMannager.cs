using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMannager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;  //BGM 사운드 조절
    [SerializeField] private Slider sfxSlider;  //SFX 사운드 조절
    [SerializeField] private AudioMixer mixer;


    public void BGMChange(float volume)
    {
        mixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
    }
    
    public void SFXChange(float volume)
    {
        mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
    }

    private void Awake()
    {
        bgmSlider.onValueChanged.AddListener(BGMChange);
        sfxSlider.onValueChanged.AddListener(SFXChange);
    }

    private void OnEnable()
    {
        Debug.Log("OnEnable");
        bgmSlider.value = mixer.GetFloat("BGM", out float bgmVolume) ? Mathf.Pow(10, bgmVolume / 20) : 1;
        sfxSlider.value = mixer.GetFloat("SFX", out float sfxVolume) ? Mathf.Pow(10, sfxVolume / 20) : 1;
    }
}
