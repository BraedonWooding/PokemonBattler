using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using EnumLibrary;

public class Battler : MonoBehaviour
{
    public static List<Pokemon> allyPokemon = new List<Pokemon>();
    public static int currentAllyPokemonID = 0;
    public static List<Pokemon> enemyPokemon = new List<Pokemon>();
    public static int currentEnemyPokemonID = 0;

    public static ScenarioBattle onCompleteAction;

    private static Battlers _currentTurn;

    public static Battlers currentTurn
    {
        set
        {
            _currentTurn = value;
            Debug.Log(value);
        }
        get
        {
            return _currentTurn;
        }
    }
    public static Text displayText;
    public static GameObject battlerPanel;
    [SerializeField]
    public static ActionButton[]
        buttons = new ActionButton[4];
    public static List<Items> items = new List<Items>();
    public GameObject buttonPanel;
    private bool pokeDied = false;
    public bool cancelAllPresses = false;
    [SerializeField]
    public AISystem
        aiCore;
    public bool startGame = false;

    public static Dictionary<string, Types> types = new Dictionary<string, Types>();
    public static Dictionary<string, Status> statuses = new Dictionary<string, Status>();

    public static Queue<string> textToDisplay = new Queue<string>();

    public static bool running = false;

    bool loss = false;

    private HPBar allyHPBar;
    private HPBar enemyHPBar;

    public Sprite allyTrainerSprite;
    public Sprite enemyTrainerSprite;

    private Image allyTrainerImg;
    private Image enemyTrainerImg;

    private Image allyPokemonImg;
    private Image enemyPokemonImg;

    private static bool activated = false;

    public static Battler instance;

    public Dictionary<Types, Dictionary<DictionaryData, Types>> weaknessTable = new Dictionary<Types, Dictionary<DictionaryData, Types>>();

    private Battlers oldBattler = Battlers.ally;

    public enum DictionaryData
    {
        weakness,
        strength,
        immunity
    }

    public void ClearText ()
    {
        textToDisplay.Clear();
    }

    public static Types GetType(string typeName)
    {
        if (types.ContainsKey(typeName))
            return types[typeName];
        else
            return null;
    }

    public static List<Types> GetTypes(string[] typeNames)
    {
        List<Types> returnTypes = new List<Types>();

        for (int i = 0; i < typeNames.Length; i++)
        {
            if (types.ContainsKey(typeNames[i]))
                returnTypes.Add(types[typeNames[i]]);
        }

        return returnTypes;
    }

    public static Status GetStatus(string statusName)
    {
        if (statuses.ContainsKey(statusName))
            return statuses[statusName];
        else
            return null;
    }

    public static List<Status> GetStatuses(string[] statusNames)
    {
        List<Status> returnTypes = new List<Status>();

        for (int i = 0; i < statusNames.Length; i++)
        {
            if (statuses.ContainsKey(statusNames[i]))
                returnTypes.Add(statuses[statusNames[i]]);
        }

        return returnTypes;
    }

    void Start()
    {
        currentTurn = Battlers.ally;

        if (!activated)
            instance = this;
        else
            Destroy(this);

        displayText = GameObject.Find("BattlerDisplayText").GetComponent<Text>();

        battlerPanel = GameObject.Find("Battler/Panel/BattlerPanel");

        allyHPBar = GameObject.Find("AllyHp").GetComponent<HPBar>();
        enemyHPBar = GameObject.Find("EnemyHP").GetComponent<HPBar>();

        allyTrainerImg = GameObject.Find("AllyPokemonTrainer").GetComponent<Image>();
        enemyTrainerImg = GameObject.Find("EnemyPokemonTrainer").GetComponent<Image>();

        allyPokemonImg = GameObject.Find("AllyPokemon").GetComponent<Image>();
        enemyPokemonImg = GameObject.Find("EnemyPokemon").GetComponent<Image>();

        allyTrainerImg.sprite = allyTrainerSprite;
        enemyTrainerImg.sprite = enemyTrainerSprite;

        aiCore = GameObject.Find("_SCRIPTS_STARTUP_").GetComponent<AISystem>();

        buttonPanel = GameObject.Find("ButtonPanel");

        for (int i = 0; i < 4; i++)
        {
            buttons[i] = GameObject.Find("Action" + (i + 1)).GetComponent<ActionButton>();
        }

        battlerPanel.SetActive(true);

        aiCore = GameObject.Find("_SUPER_STARTUP_").GetComponent<AISystem>();
    }

    public static IEnumerator ChangeTextAsNeeded(string text) // <- make this function a coroutine
    {
        running = true;

        text = text.Replace("/(%Turn%)/", currentTurn.ToString());

        text = text.Replace("/(%!Turn%)/", ((currentTurn == Battlers.ally) ? Battlers.enemy : Battlers.ally).ToString());

        text = text.Replace("/(%TurnPokemon%)/", ((currentTurn == Battlers.ally) ? allyPokemon[currentAllyPokemonID].pName : enemyPokemon[currentEnemyPokemonID].pName));

        text = text.Replace("/(%!TurnPokemon%)/", ((currentTurn == Battlers.ally) ? enemyPokemon[currentEnemyPokemonID].pName : allyPokemon[currentAllyPokemonID].pName));

        displayText.text = text;
        yield return 0;

        while (!Input.GetKeyDown(KeyCode.Space))
        {
            instance.cancelAllPresses = true;
            yield return 0;
        }

        if (displayText.text == text)
        {
            running = false;
        }
    }

    void Update()
    {
        if (startGame)
        {
            if (textToDisplay.Count > 0 || running)
            {
                cancelAllPresses = true;
            }
            else if (!running && textToDisplay.Count == 0)
            {
                if (currentTurn != oldBattler)
                {
                    if (currentTurn == Battlers.ally)
                    {
                        displayText.text = "Your Turn";
                    }
                    else if (currentTurn == Battlers.enemy)
                    {
                        displayText.text = "Enemy is thinking";
                    }
                }
            }

            if (textToDisplay.Count > 0)
            {
                displayText.gameObject.SetActive(true);
                if (!running)
                    StartCoroutine(ChangeTextAsNeeded(textToDisplay.Dequeue()));
            }

            allyHPBar.hp = allyPokemon[currentAllyPokemonID].pHp;
            allyHPBar.maxHp = allyPokemon[currentAllyPokemonID].pMaxHp;

            enemyHPBar.hp = enemyPokemon[currentEnemyPokemonID].pHp;
            enemyHPBar.maxHp = enemyPokemon[currentEnemyPokemonID].pMaxHp;

            allyPokemonImg.sprite = allyPokemon[currentAllyPokemonID].pSprite;
            enemyPokemonImg.sprite = enemyPokemon[currentEnemyPokemonID].pSprite;

            if (currentTurn != oldBattler && !running)
            {
                if (currentTurn == Battlers.ally)
                {
                    //Do Ally Scripts
                    if (allyPokemon[currentAllyPokemonID].pHp <= 0)
                    {
                        pokeDied = true;
                    }
                }
                else if (currentTurn == Battlers.enemy)
                {
                    //Do Ally Scripts
                    if (enemyPokemon[currentEnemyPokemonID].pHp <= 0)
                    {
                        pokeDied = true;
                    }
                    else
                    {
                        aiCore.Run("Function_Choose_Action");
                        oldBattler = currentTurn;
                    }
                }
                oldBattler = currentTurn;
            }

            if (pokeDied)
            {
                cancelAllPresses = true;
                int a = 0;
                int e = 0;

                if (currentTurn == Battlers.ally)
                {
                    for (int i = 0; i < allyPokemon.Count; i++)
                    {
                        if (allyPokemon[i].pHp > 0)
                            a++;
                    }
                }
                else
                {
                    for (int i = 0; i < enemyPokemon.Count; i++)
                    {
                        if (enemyPokemon[i].pHp > 0)
                            e++;
                    }
                }
                if (a <= 0)
                {
                    //Do Death statement then return MAKE SURE YOU EITHER RELOAD SCENE/TURN CANCEL ALL PRESSES OFF!!!!
                    loss = true;
                    Application.LoadLevel("Scenario");
                    return;
                }
                else if (e <= 0)
                {
                    loss = false;
                    Application.LoadLevel("Scenario");
                    return;
                }

                if (currentTurn == Battlers.ally)
                {
                    //Atleast one pokemon surviving
                    cancelAllPresses = false;
                    ButtonPressed(Battlers.ally, ActionButton.Choices.pokemon);
                    cancelAllPresses = true;
                }
                else if (currentTurn == Battlers.enemy)
                {
                    Debug.LogWarning("Fix this later");
                    Application.LoadLevel("Starting");
                    return;
                }
            }
        }
    }

    public void OnLevelWasLoaded()
    {
        if (Application.loadedLevelName == "Scenario")
        {
            ScenarioLog log = FindObjectOfType<ScenarioLog>();

            if (onCompleteAction.sbLossAction == ScenarioButtonAction.NodeString)
            {
                if (loss)
                    log.NodeString(onCompleteAction.sbLossString);
                else
                    log.NodeString(onCompleteAction.sbLossString);
            }
            else if (onCompleteAction.sbLossAction == ScenarioButtonAction.NodeX)
            {
                if (loss)
                    log.NodeInt(onCompleteAction.sbLossInt);
                else
                    log.NodeInt(onCompleteAction.sbWinInt);
            }
        }
    }

    public void RunButton()
    {
        ButtonPressed(Battlers.ally, ActionButton.Choices.run);
    }

    public void AttackButton()
    {
        ButtonPressed(Battlers.ally, ActionButton.Choices.attack);
    }

    public void BagButton()
    {
        ButtonPressed(Battlers.ally, ActionButton.Choices.bag);
    }

    public void PokemonButton()
    {
        ButtonPressed(Battlers.ally, ActionButton.Choices.pokemon);
    }

    public void ButtonPressed(Battlers sender, ActionButton.Choices type)
    {
        if (sender == currentTurn || !cancelAllPresses && startGame)
        {
            switch (type)
            {
                case ActionButton.Choices.run:
                    if (sender == Battlers.ally)
                    {
                        foreach (Status status in allyPokemon[currentAllyPokemonID].pStatus.OrderBy(x => x.sPriority))
                        {
                            if (status.sType == Status.StatusType.beforeRun || status.sType == Status.StatusType.beforeAnyRun)
                            {
                                try
                                {
                                    aiCore.Run(status.sFunctionName, null, null, null, null, status);
                                }
                                catch (MyException ex)
                                {
                                    textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                    Debug.Log(ex.Message);
#endif
                                }
                            }
                        }

                        if (enemyPokemon[currentEnemyPokemonID].pStatus.Count > 0)
                        {
                            foreach (Status status in enemyPokemon[currentEnemyPokemonID].pStatus.OrderBy(x => x.sPriority))
                            {
                                if (status.sType == Status.StatusType.beforeAnyRun || status.sType == Status.StatusType.beforeEnemyRun)
                                {
                                    try
                                    {
                                        aiCore.Run(status.sFunctionName, null, null, null, null, status);
                                    }
                                    catch (MyException ex)
                                    {
                                        textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                        Debug.Log(ex.Message);
#endif
                                    }
                                }
                            }
                        }

                        float chance = UnityEngine.Random.Range(0.0f, 1.0f);

                        if (chance <= allyPokemon[currentAllyPokemonID].pSpeed / enemyPokemon[currentEnemyPokemonID].pSpeed)
                            //Run
                            displayText.text = allyPokemon[currentAllyPokemonID].pName + " ran away from " + enemyPokemon[currentEnemyPokemonID];
                    }
                    break;

                case ActionButton.Choices.bag:
                    //Setup buttons
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        buttons[i].aName = items[i].iName;
                        buttons[i].currentChoice = ActionButton.Choices.bag;
                        buttons[i].id = i;
                    }
                    if (sender == Battlers.ally)
                    {
                        battlerPanel.SetActive(false);
                    }
                    else
                    {
                        //Run AI script
                        battlerPanel.SetActive(true);
                        aiCore.Run("Function_Action_Bag_Choices");
                    }
                    break;

                case ActionButton.Choices.attack:
                    //Battler has pressed attack button to say he wants to attack
                    //setup Buttons fix this if serious
                    for (int i = 0; i < buttons.Length; i++)
                    {
                        if (allyPokemon[currentAllyPokemonID].pMoves.Count > i)
                            buttons[i].aName = allyPokemon[currentAllyPokemonID].pMoves[i].mName;
                        buttons[i].currentChoice = ActionButton.Choices.attack;
                        buttons[i].id = i;
                    }

                    if (sender == Battlers.ally)
                    {
                        battlerPanel.SetActive(false);
                    }
                    else
                    {
                        //Run AI script
                        //Argument System run
                        battlerPanel.SetActive(true);
                        aiCore.Run("Function_Action_Attack_Choices");
                    }
                    break;

                case ActionButton.Choices.pokemon:
                    {
                        //Setup buttons
                        for (int i = 0; i < buttons.Length; i++)
                        {
                            buttons[i].aName = allyPokemon[i].pName;
                            buttons[i].currentChoice = ActionButton.Choices.pokemon;
                            buttons[i].id = i;
                        }
                        if (sender == Battlers.ally)
                        {
                            battlerPanel.SetActive(false);
                        }
                        else
                        {
                            //Run AI script
                            battlerPanel.SetActive(true);
                            aiCore.Run("Function_Action_Pokemon_Choices");
                        }
                        break;
                    }
            }
        }
    }

    public enum Battlers
    {
        ally = 0,
        enemy = 1,
        none = 2
    }
}