using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    public AudioClip music;
    public bool loop;
    public bool dontDestroyOnLoad = true;

    private void Awake()
    {
        GetComponent<AudioSource>().playOnAwake = false;
    }

    private void Start()
    {
        if (instance)
        {
            if (instance.music == music)
            {
                Destroy(this);
                return;
            }
            else
            {
                Destroy(instance.gameObject);
            }
        }

        Play();
        instance = this;
    }

    public void Play()
    {
        if (dontDestroyOnLoad) DontDestroyOnLoad(this);
        AudioSource source = GetComponent<AudioSource>();

        source.clip = music;
        source.loop = loop;
        source.Play();
    }
}
