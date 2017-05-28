using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    [SerializeField]
    private List<AudioClip> audioFiles = new List<AudioClip>();

    public AudioSource source;

    public List<AudioClip> playedAudio = new List<AudioClip>();

    private static MusicManager m_Instance;

    private const bool installMusic = true;

    public static Settings musicSettings;

    private bool paused = false;

    private static MusicManager Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = (new GameObject("MusicManager")).AddComponent<MusicManager>();
            }
            return m_Instance;
        }
    }

    public enum Settings
    {
        dontRepeat2,
        dontRepeatTillAllSongs,
        repeat
    }

    public static void AudioVolume(float volume, bool musicOn)
    {
            Instance.source.volume = volume / 100;
            Instance.source.mute = musicOn;
    }

    public static void ResetMusic()
    {
        Instance.playedAudio.Clear();
    }

    void Awake()
    {
        DontDestroyOnLoad(this);
        m_Instance = this;
        source = GetComponent<AudioSource>();
        audioFiles = Resources.LoadAll<AudioClip>("Audio/1Batch").ToList();
        if (installMusic)
            StartCoroutine(LoadAudio());
    }

    IEnumerator LoadAudio()
    {
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/2Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/3Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/4Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/5Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/6Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/7Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/8Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/9Batch"));
        yield return new WaitForEndOfFrame();
        audioFiles.AddRange(Resources.LoadAll<AudioClip>("Audio/10Batch"));
        yield break;
    }

    public static void ChangeSettings (int newValue)
    {
        musicSettings = (Settings)newValue;
    }

    // Update is called once per frame
    void Update()
    {
        if ((!Instance.paused && !Instance.source.isPlaying) || Input.GetKeyDown(KeyCode.F8))
        {
            if (musicSettings == Settings.dontRepeat2)
                InsertRandomClip(source.clip);
            else if (musicSettings == Settings.dontRepeatTillAllSongs)
                InsertRandomClipNotPlayedBefore(playedAudio);
            else
                InsertRandomClip(null);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            //Just re run clip
            if (source.time <= 2 && playedAudio.Count > 1)
            {
                //Play previous clip
                source.Stop();
                source.clip = playedAudio[playedAudio.Count - 2];
                source.Play();
            }
            else
            {
                source.time = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            if (source.isPlaying)
            {
                source.Pause();
                paused = true;
            }
            else
            {
                source.UnPause();
                paused = false;
            }
        }
    }

    void InsertRandomClipNotPlayedBefore (List<AudioClip> except)
    {
        List<AudioClip> newAudio = audioFiles.Except(except).ToList();

        int clipRandom = Random.Range(0, newAudio.Count);

        Instance.source.clip = newAudio[clipRandom];
        playedAudio.Add(Instance.source.clip);
        Instance.source.Play();
    }

    void InsertRandomClip(AudioClip except = null)
    {
        int clipRandom = Random.Range(0, audioFiles.Count);

        if (except != null && audioFiles[clipRandom] == except)
        {
            InsertRandomClip(except);
            return;
        }

        Instance.source.clip = audioFiles[clipRandom];
        playedAudio.Add(audioFiles[clipRandom]);
        Instance.source.Play();
    }
}