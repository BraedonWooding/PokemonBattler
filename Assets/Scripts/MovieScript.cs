using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(GUITexture))]
public class MovieScript : MonoBehaviour
{
    MovieTexture movie;
    AudioSource movieAudio;
    GUITexture videoGUItex;

    public string movieName;

    void Awake()
    {
        videoGUItex = GetComponent<GUITexture>();
        movie = (MovieTexture)Resources.Load(movieName);
        movieAudio = GetComponent<AudioSource>();
        Debug.Log(movie.name);
        movieAudio.clip = movie.audioClip;

        videoGUItex.pixelInset = new Rect(Screen.width / 2, Screen.height / 2, 0, 0);
    }

    void Start ()
    {
        videoGUItex.texture = movie;
        movie.Play();
        movieAudio.Play();

        AutoFade.LoadLevel(1, 9f, 1f, Color.white);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AutoFade.ForceLoad();
        }
    }
}