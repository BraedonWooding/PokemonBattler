using UnityEngine;
using System.Collections;

public class Refresh : MonoBehaviour {

    public void RefreshLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Go()
    {
        GameObject.Find("_SCRIPTS_STARTUP_").GetComponent<SetupChoices>().RunSimulation();
    }
}