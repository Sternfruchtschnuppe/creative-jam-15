using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject SettingsPanel;

    public Slider volumeSlider;
    void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("volume", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnVolumeSliderChange()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
    public void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnSettings()
    {
        MenuPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }
    public void OnBack()
    {
        MenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);

    }
    public void OnExit()
    {
        Application.Quit();
    }
}
