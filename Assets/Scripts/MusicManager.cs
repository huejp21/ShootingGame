using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTheme;
    public AudioClip menuTheme;

    string sceneNmae;

    // Use this for initialization
    void Start()
    {
        //AudioManager.instance.PlayMusic(menuTheme, 2);
        OnLevelWasLoaded(0);
    }

    void OnLevelWasLoaded(int level)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if (newSceneName != sceneNmae)
        {
            sceneNmae = newSceneName;
            Invoke("PlayMusic", 0.2f);
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;
        switch (sceneNmae)
        {
            case "Menu":
                clipToPlay = menuTheme;
                break;
            case "Game":
                clipToPlay = mainTheme;
                break;
            default:
                break;
        }

        if (clipToPlay != null)
        {
            AudioManager.instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }

    // Update is called once per frame
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        AudioManager.instance.PlayMusic(mainTheme, 3);
    //    }
    //}
}
