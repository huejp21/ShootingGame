using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBehaviour {

    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }

    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

    void Awake()
    {
        foreach (SoundGroup soundGroup in soundGroups)
        {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }

    public AudioClip GetClipFromName(string name)
    {
        if (groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

	//// Use this for initialization
	//void Start () {
		
	//}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}
