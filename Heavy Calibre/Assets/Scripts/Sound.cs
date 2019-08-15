using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{

    private AudioSource source;
    // text field for clip name
    public string clipName; 

    public AudioClip clip;
    public AudioMixerGroup audioMixerGroup;

    // adds slider to float
    [Range (0f,1f)]
    public float volume;
    // adds float to slider
    [Range (0f,3f)]
    public float pitch;

    // sets default sate for the bool
    public bool loop =  false;
    public bool playOnAwake = false;
    public bool oneShot = false;

    [Range(0f, 1f)]
    public float spatial = 0;

    public float minDistance3Dsound = 1f, maxDistance3DSound = 5f;

    // adds editable sections to the array 
    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.volume = volume;
        source.playOnAwake = playOnAwake;
        source.loop = loop;
        source.outputAudioMixerGroup = audioMixerGroup;
        source.spatialBlend = spatial;
        source.minDistance = minDistance3Dsound;
        source.maxDistance = maxDistance3DSound;
    }

    public void Play()
    {
        if (oneShot == true)
        {
            if(!source.isPlaying)
                source.Play();
        }
        else source.Play();
    }


    public void Play(AudioSource _audioSource)
    {
        SetSource(_audioSource);

        if (oneShot == true)
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
        }
        else _audioSource.Play();
    }
}
