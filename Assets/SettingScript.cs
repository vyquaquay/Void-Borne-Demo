using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{

    [SerializeField] AudioMixer audioMixer;
    public float volumeValue;
    public Slider volumeSlider;
    public Dropdown graphicdropdown;
    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
        volumeValue = _volume;
    }

    public void setQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void setFullScreen(bool _fullscreen)
    {
        Screen.fullScreen = _fullscreen;
    }

    public void Quit()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        //graphicdropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        audioMixer.SetFloat("Volume", volumeValue);
        PlayerPrefs.SetFloat("Volume", volumeValue);
    }
}
