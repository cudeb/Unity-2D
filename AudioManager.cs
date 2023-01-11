using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;
    private AudioSource source;

    public float volume;
    public bool loop;

    public void SetSource(AudioSource newSource)
    {
        source = newSource;
        source.clip = clip;
        source.volume = volume;
        source.loop = loop;
    }

    public void SetVolume()
    {
        source.volume = volume;
    }

    public void Play()
    {
        source.Play();
    }

    public void Stop()
    {
        source.Stop();
    }

    public void SetLoop()
    {
        source.loop = true;
    }

    public void SetLoopCancel()
    {
        source.loop = false;
    }
}

public class AudioManager : MonoBehaviour
{
    static public AudioManager instance;
    
    [SerializeField]
    public Sound[] sounds;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<sounds.Length;i++)
        {
            GameObject soundObject = new GameObject("사운드 파일 이름 : " + i + sounds[i].name);
            sounds[i].SetSource(soundObject.AddComponent<AudioSource>()) ;
            soundObject.transform.SetParent(this.transform);
        }
    }

    public void Play(string _name)
    {
        for(int i =0;i<sounds.Length;i++)
        {
            if(_name == sounds[i].name)
            {
                sounds[i].Play();
                return;
            }
        }
    }

    public void Stop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].Stop();
                return;
            }
        }
    }

    public void SetLoop(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoop();
                return;
            }
        }
    }

    public void SetLoopCancel(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].SetLoopCancel();
                return;
            }
        }
    }

    public void SetVolume(string _name, float _volume)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (_name == sounds[i].name)
            {
                sounds[i].volume = _volume;
                sounds[i].SetVolume();
                return;
            }
        }
    }
}
