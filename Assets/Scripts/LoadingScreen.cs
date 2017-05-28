using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour {

    public Text text;
    public Image circleImage;

    private AsyncOperation async = null; // When assigned, load is in progress.

    public string levelName;

    void Start ()
    {
        StartCoroutine(LoadALevel(levelName));
    }

    private IEnumerator LoadALevel(string levelName)
    {
        async = Application.LoadLevelAsync(levelName);
        yield return async;
    }
    void OnGUI()
    {
        if (async != null)
        {
            circleImage.fillAmount = async.progress;
            text.text = async.progress * 100 + "%";
        }
    }
}