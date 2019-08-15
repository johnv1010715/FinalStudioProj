using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/* 
    SCRIPT AUTHOR/S:
    William Roberts with additions by Laurence Valentini

    SCRIPT DESCRIPTION: 
    This script should be used to easily find and play specific sound effects. It should stay persistent throughout all scenes.

    To use this:
    // AudioManager audiomanager;
    // in start method { audiomanager = AudioManager.instance; }
    // to play sound audioManager.PlaySound("Sound Name")
    // or to play sound from a specific audiosource audioManager.PlaySound("Sound Name", audiosource)

*/

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance; // a static instance of this script

    [SerializeField]
    Sound[] sound; // an array of sound class which is a container for sound effects and their audio properties

    public bool isMuted = false; // is the sound muted





	// Use this for initialization
	void Awake ()
    {
        // this block of code ensures that the object does not destroy itself or become duplicated
        DontDestroyOnLoad(gameObject);

		if (instance == null)
        {
            instance = this;
        }
        else if ( instance != this )
        {
            Destroy(gameObject);
        }
	}




    void Start()
    {
        // go through each sound in the sound array and greate a gameobject parented to the audio manager with an audio source
        for (int i = 0; i < sound.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sound[i].clipName);
            _go.transform.SetParent(this.transform);
            sound[i].SetSource(_go.AddComponent<AudioSource>());
        }

        // play the theme music
        PlaySound("Theme");
    }
    


    // this method finds which sound is to be played and plays it using the created audio source
    public void PlaySound(string _name)
    {
		for (int i = 0; i < sound.Length; i++)
        {
            if (sound[i].clipName == _name)
            {
                sound[i].Play();
                return;
            }
        }
	}

    // this method finds which sound is to be played and plays it using an already existing audio source that is passed in as a parameter
    public void PlaySound(string _name, AudioSource _audioSource)
    {
        for (int i = 0; i < sound.Length; i++)
        {
            if (sound[i].clipName == _name)
            {
                sound[i].Play(_audioSource);
                return;
            }
        }
    }
}
