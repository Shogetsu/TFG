using UnityEngine;
using System.Collections;


[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    private AudioSource source;

    [Range(0f, 1f)]
    public float vol = 0.7f;
    [Range(0.5f, 1.5f)]
    public float pitch = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;


    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
    }

    public void Play()
    {
        source.volume = vol * (1 + Random.Range(-randomVolume/2f, randomVolume/2f));
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.Play();
    }


    public void Stop()
    {
        /* source.volume = vol * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f));
         source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));*/
      /*  while (source.volume > 0.1f)
        {
            source.volume -= 0.01f * Time.deltaTime;
        }*/
        source.Stop();
        
        //StartCoroutine(fadeOut(source));
    }

    public AudioSource GetSource()
    {
        return source;
    }


}

public static class AudioFadeOut
{
    public static IEnumerator fadeOut(AudioSource _source)
    {
        float t = _source.volume;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            _source.volume = t;
            yield return new WaitForSeconds(0f);
        }
        _source.volume = 0.0f;
        _source.Stop();
        _source.loop = false;
    }

}

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    [SerializeField]
    Sound[] sounds;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioManager in the scene. ");
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        for(int i=0; i<sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name.Equals(_name))
            {
                sounds[i].Play();
                if (sounds[i].name.Equals("Footstep") || sounds[i].name.Equals("Sea")) sounds[i].GetSource().loop = true;

                return;
            }
        }

        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list," + _name);

    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name.Equals(_name))
            {
                //sounds[i].Stop();
                StartCoroutine(AudioFadeOut.fadeOut(sounds[i].GetSource()));
                return;
            }
        }

        //no sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list," + _name);
    }
}
