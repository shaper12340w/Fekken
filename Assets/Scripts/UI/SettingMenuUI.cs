using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SettingMenuUI : MonoBehaviour
    {
        [SerializeField] private Slider bgmSlider; //BGM 사운드 조절
        [SerializeField] private Slider sfxSlider; //SFX 사운드 조절
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private SingleButtonSelector singleButtonSelector;

        private void Awake()
        {
            bgmSlider.onValueChanged.AddListener(BGMChange);
            sfxSlider.onValueChanged.AddListener(SFXChange);

        }

        private void Update()
        {
            if(singleButtonSelector != null)
                if(singleButtonSelector.GetSelectedButtonText() != null)
                    PlayerPrefs.SetString("Theme", singleButtonSelector.GetSelectedButtonText());

        }

        private void OnEnable()
        {
            Debug.Log("OnEnable");
            bgmSlider.value = mixer.GetFloat("BGM", out var bgmVolume) ? Mathf.Pow(10, bgmVolume / 20) : 1;
            sfxSlider.value = mixer.GetFloat("SFX", out var sfxVolume) ? Mathf.Pow(10, sfxVolume / 20) : 1;
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Canvas").transform.Find("ButtonList").Find(PlayerPrefs.GetString("Theme")).gameObject);
        }

        public void BGMChange(float volume)
        {
            mixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        public void SFXChange(float volume)
        {
            mixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }
    }
}