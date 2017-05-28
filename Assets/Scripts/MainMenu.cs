using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class MainMenu : MonoBehaviour
{

    [SerializeField]
    private GameObject exitMenu;

    [SerializeField]
    private GameObject deleteMenuDANGEROUS;

    [SerializeField]
    private GameObject restartMenuDANGEROUS;

    [SerializeField]
    private InputField deleteInputFieldDANGEROUS;

    [SerializeField]
    private Text deleteCountdownDANGEROUS;

    [SerializeField]
    private GameObject optionsMenu;

    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Slider settingsSlider;

    [SerializeField]
    private Toggle toggle;

    private GameXMLSerializer file;

    void Start()
    {
        bool musicOn = true;

        file = GameXMLSerializer.Load(Application.streamingAssetsPath + "/Settings/Settings.xml");

        musicOn = file.volumeOn;

        toggle.isOn = musicOn;

        float volume = file.volume;
        slider.value = volume;
        MusicManager.AudioVolume(volume, musicOn);

        int settings = file.settings.GetHashCode();
        settingsSlider.value = settings;
        MusicManager.ChangeSettings(settings);
    }

    public void ResetMusic()
    {
        MusicManager.ResetMusic();
    }

    public void ExitGame()
    {
        exitMenu.SetActive(true);
    }

    public void ReallyExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        Application.LoadLevel("Scenario");
    }

    public void Options()
    {
        optionsMenu.SetActive(true);
    }

    public void Save(bool quit)
    {
        file.volume = slider.value;
        file.volumeOn = toggle.isOn;
        file.settings = (MusicManager.Settings)settingsSlider.value;

        file.Save(Application.dataPath + "/BattlerData/Settings/Settings.xml");
        file = GameXMLSerializer.Load(Application.dataPath + "/BattlerData/Settings/Settings.xml");

        bool musicOn;
        
        musicOn = file.volumeOn;

        toggle.isOn = musicOn;

        MusicManager.ChangeSettings((int)settingsSlider.value);

        float volume = file.volume;
        slider.value = volume;
        MusicManager.AudioVolume(volume, musicOn);

        int settings = file.settings.GetHashCode();
        settingsSlider.value = settings;
        MusicManager.ChangeSettings(settings);

        if (quit)
        {
            GameObject.Find("OptionsMenu").SetActive(false);
        }
    }

    public void ResetGameData ()
    {
        deleteMenuDANGEROUS.SetActive(true);
    }

    public void ActuallyDeleteData ()
    {
        if (deleteInputFieldDANGEROUS.text.ToLower() == "yes" || deleteInputFieldDANGEROUS.text.ToLower() == @"""yes""" || deleteInputFieldDANGEROUS.text.ToLower() == @"""yes" || deleteInputFieldDANGEROUS.text.ToLower() == @"yes""")
        {
            PlayerPrefs.SetFloat("Version_Number", 0);

            restartMenuDANGEROUS.SetActive(true);
            StartCoroutine(CountDown());
        }
        else
        {
            deleteMenuDANGEROUS.SetActive(false);
        }
    }

    IEnumerator CountDown ()
    {
        float seconds = 3;
        while (seconds > 0)
        {
            seconds -= 0.1f;
            deleteCountdownDANGEROUS.text = seconds.ToString("0.0");
            deleteCountdownDANGEROUS.fontSize = (int)(230 - (seconds*50));
            yield return new WaitForSeconds(0.1f);
        }

        deleteCountdownDANGEROUS.text = "NOW!";
        yield return new WaitForSeconds(0.25f);
        Debug.Log("Done");
        Application.Quit();
    }
}