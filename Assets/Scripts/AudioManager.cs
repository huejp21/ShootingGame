using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public enum AudioChannel
    {
        Master,
        Sfx,
        Music,
    };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    AudioSource sfx2DSource;
    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    public static AudioManager instance;

    Transform audioListener;
    Transform playerT;

    SoundLibrary library;

    void OnLevelWasLoaded(int index)
    {
        if (playerT == null)
        {
            if (FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }
        }
    }

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            library = GetComponent<SoundLibrary>();

            int audioSourceCount = 2;

            musicSources = new AudioSource[audioSourceCount];
            for (int i = 0; i < audioSourceCount; i++)
            {
                GameObject newMusicSource = new GameObject("Music source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }
            GameObject newSfx2DSource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if (FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }

            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1.0f);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1.0f);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1.0f);
            PlayerPrefs.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (playerT != null)
            {
                audioListener.position = playerT.position;
            }
        }
        catch (System.Exception ex)
        {

        }

    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }


        for (int i = 0; i < musicSources.Length; i++)
        {
            musicSources[i].volume = musicVolumePercent * masterVolumePercent;
        }

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);

    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;
        while (percent < 1.0f)
        {
            percent += Time.deltaTime * 1.0f / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
	//// Use this for initialization
	//void Start () {
		
	//}
	

}
