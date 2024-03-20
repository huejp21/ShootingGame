using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;

    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle[] userLevelToggles;
    public int[] screenWidths;
    public Toggle fullScreenToggle;
    int activeScreenResolutionIndex;
    int activeUserLevelIndex;

    // Use this for initialization
    void Start()
    {
        activeUserLevelIndex = PlayerPrefs.GetInt("User Level");
        activeScreenResolutionIndex = PlayerPrefs.GetInt("screen resolution index");
        bool isFullscreen = PlayerPrefs.GetInt("fullscreen") == 1 ? true : false;


        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].isOn = (i == activeScreenResolutionIndex);
        }

        for (int i = 0; i < userLevelToggles.Length; i++)
        {
            userLevelToggles[i].isOn = (i == activeUserLevelIndex);
        }

        Spawner.userLevel = (Spawner.UserLevel)activeScreenResolutionIndex;
        fullScreenToggle.isOn = isFullscreen;
        

    }

    public int GetSavedUserLevel()
    {
        return activeUserLevelIndex;
    }

    public void Play()
    {
        ScoreKeeper.ResetScore();
        SceneManager.LoadScene("Game"); 
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OptionsMenu()
    {
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }

    public void MainMenu()
    {
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetUserLevel(int selection)
    {
        if (userLevelToggles[selection].isOn)
        {
            activeUserLevelIndex = selection;
            Spawner.userLevel = (Spawner.UserLevel)selection;
            PlayerPrefs.SetInt("User Level", activeUserLevelIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetScreenResolution(int selection)
    {
        if (resolutionToggles[selection].isOn)
        {
            activeScreenResolutionIndex = selection;
            float aspectRatio = 16.0f / 9.0f;
            Screen.SetResolution(screenWidths[selection], (int)(screenWidths[selection] / aspectRatio), false);
            PlayerPrefs.SetInt("screen resolution index", activeScreenResolutionIndex);
            PlayerPrefs.Save();
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        isFullscreen = fullScreenToggle.isOn;

        for (int i = 0; i < resolutionToggles.Length; i++)
        {
            resolutionToggles[i].interactable = !isFullscreen;
        }

        if (isFullscreen)
        {
            Resolution[] allResolutions = Screen.resolutions;
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else
        {
            SetScreenResolution(activeScreenResolutionIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullscreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(float value)
    {
        value = volumeSliders[0].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(float value)
    {
        value = volumeSliders[1].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(float value)
    {
        value = volumeSliders[2].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }




    //// Update is called once per frame
    //void Update () {

    //}
}
