using UnityEngine;
using System.Collections;
using EnumLibrary;
using UnityEngine.UI;
using System;
using System.Linq;

public class ScenarioButton : MonoBehaviour
{

    public ScenarioButtonAction action;
    public string title;
    public ScenarioNodeButton button;
    public ScenarioLog log;

    void Start ()
    {
        GetComponentInChildren<Text>().text = title;
    }

    public void Clicked()
    {
        if (action == ScenarioButtonAction.NodeString)
        {
            log.NodeString(button.bString);
        }
        else if (action == ScenarioButtonAction.NodeX)
        {
            log.NodeInt(button.bInt);
        }
        else if (action == ScenarioButtonAction.StartBattle)
        {
            transform.parent = null;
            DontDestroyOnLoad(this);
            Application.LoadLevel("Starting");
        }
    }

    void OnLevelWasLoaded()
    {
        if (Application.loadedLevelName == "Battler")
        {
            //Level Loaded now input the data
            Battler battler = GameObject.Find("_SCRIPTS_BATTLER_").GetComponent<Battler>();

            Startup startup = FindObjectOfType<Startup>();

            foreach (var valuePair in button.bBattle.sbAllyNames)
            {
                Battler.allyPokemon.Add(startup.pokemon[valuePair]);
            }

            foreach (var valuePair in button.bBattle.sbEnemyNames)
            {
                Battler.enemyPokemon.Add(startup.pokemon[valuePair]);
            }

            battler.allyTrainerSprite = startup.trainerSprites.ElementAt(0).Value;
            battler.enemyTrainerSprite = startup.trainerSprites.ElementAt(1).Value;

            Battler.statuses = startup.statuses;
            Battler.types = startup.types;

            battler.startGame = true;

            Destroy(this);
        }
    }
}