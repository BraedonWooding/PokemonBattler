using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EnumLibrary;

public class EditorResetStreamingAssets : EditorWindow
{
    static List<string> basicSettingsString = new List<string>() { "Delete Battler Data", "Delete Streaming Assets", "Delete Settings" };

    static Dictionary<string, bool> basicSettings = new Dictionary<string, bool>();

    static List<string> advancedSettingsString = new List<string>() { "Create Items Example", "Create Unit Example", "Create Type Example", "Create Status Example", "Create Move Example", "Create Scenarios Example", "Create Function Example", "Create Settings" };

    static Dictionary<string, bool> advancedSettings = new Dictionary<string, bool>();

    bool allCreateSettings = false;
    bool allDeleteSettings = false;

    [MenuItem("Unit Editor/Reset Window")]
    public static void ResetWindow()
    {
        if (!EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isCompiling)
        {
            basicSettings.Clear();
            foreach (var value in basicSettingsString)
            {
                basicSettings.Add(value, false);
            }

            advancedSettings.Clear();
            foreach (var value in advancedSettingsString)
            {
                advancedSettings.Add(value, false);
            }

            GetWindow(typeof(EditorResetStreamingAssets));
        }
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginVertical();
        GUILayout.Label("Run Reset", EditorStyles.boldLabel);
        if (GUILayout.Button("Run Reset"))
        {
            RunReset();
        }
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = 80;
        GUILayout.BeginVertical();
        GUILayout.Label("Delete Settings", EditorStyles.boldLabel);
        allDeleteSettings = EditorGUILayout.Toggle(new GUIContent("Delete All"), allDeleteSettings, EditorStyles.toggle);
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 200;
        foreach (var valuePair in basicSettingsString)
        {
            basicSettings[valuePair] = EditorGUILayout.Toggle(new GUIContent(valuePair), basicSettings[valuePair], EditorStyles.toggle);
        }

        GUILayout.EndVertical();

        GUILayout.Space(20);

        GUILayout.BeginHorizontal();

        GUILayout.FlexibleSpace();
        EditorGUIUtility.labelWidth = 80;
        GUILayout.BeginVertical();
        GUILayout.Label("Advanced Settings", EditorStyles.boldLabel);
        allCreateSettings = EditorGUILayout.Toggle(new GUIContent("Create All"), allCreateSettings, EditorStyles.toggle);
        GUILayout.EndVertical();
        GUILayout.FlexibleSpace();

        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        EditorGUIUtility.labelWidth = 200;
        foreach (var valuePair in advancedSettingsString)
        {
            advancedSettings[valuePair] = EditorGUILayout.Toggle(new GUIContent(valuePair), advancedSettings[valuePair], EditorStyles.toggle);
        }

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

    }
    [ExecuteInEditMode]
    void RunReset()
    {
        if (allDeleteSettings)
        {
            foreach (var valuePair in basicSettingsString)
            {
                basicSettings[valuePair] = true;
            }
        }

        if (basicSettings["Delete Battler Data"])
        {
            FileUtil.DeleteFileOrDirectory(Application.dataPath + "/BattlerData");
        }

        if (basicSettings["Delete Streaming Assets"])
        {
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Modding");
        }

        if (basicSettings["Delete Settings"])
        {
            FileUtil.DeleteFileOrDirectory(Application.streamingAssetsPath + "/Settings");
        }

        if (allCreateSettings)
        {
            foreach (var valuePair in advancedSettingsString)
            {
                advancedSettings[valuePair] = true;
            }
        }

        if (advancedSettings["Create Settings"])
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Settings");
            GameXMLSerializer settings = new GameXMLSerializer();
            settings.versionNumber = 0.1f;
            settings.gameName = "Beta";
            settings.settings = MusicManager.Settings.dontRepeatTillAllSongs;
            settings.volume = 50;
            settings.volumeOn = true;
            settings.Save(Application.streamingAssetsPath + "/Settings/Settings.xml");
        }

        if (advancedSettings["Create Scenarios Example"])
        {
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Scenario/GeneralScenario/ExampleScenario");
            XMLSerializerScenarios scenario = new XMLSerializerScenarios();

            scenario.scenario = new Scenario("Example_Scenario", true, true, "Made by Braedon Wooding \n Training Mission \n Hope You Enjoyed", new ScenarioFinishAction[] { ScenarioFinishAction.MainMenu, ScenarioFinishAction.OtherScenario, ScenarioFinishAction.StartMenu }, new ScenarioNode[]
            {
                new ScenarioNode("FirstNode", true, true, new ScenarioNodeButton[] {new ScenarioNodeButton("First Button", "First Button To Click :D", new Color32(0, 204, 255, 255), ScenarioButtonAction.NodeString, 0, "SecondNode")}, new string[] { "SecondNode"}, "First Node Title", new Color32(255, 255, 0, 255), "FIRST NODE :D", new Color32(50, 125, 20, 255)), new ScenarioNode("SecondNode", true, true, new ScenarioNodeButton[] { }, new string[] { }, "SECOND SET OF BUTTONS :D", new Color32(10, 200, 30, 255), "This is a message", new Color32(153, 51, 102, 255))
            }, new Color32(0, 51, 102, 255));

            scenario.Save(Application.streamingAssetsPath + "/Modding/Scenario/GeneralScenario/ExampleScenario/ExScenario.xml");

            XMLSerializer modInfo = new XMLSerializer();
            modInfo.modActive = true;
            modInfo.modName = "Default_Game_ScenarioExample";
            modInfo.versionNumber = 1;
            modInfo.Save(Application.streamingAssetsPath + "/Modding/Scenario/GeneralScenario/ExampleScenario/Info.xml");
        }

        if (advancedSettings["Create Items Example"])
        {
            #region Item Example
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Items/GeneralItems/GeneralItems");
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GeneralItems";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/Items/GeneralItems/GeneralItems/Info.xml");
            }

            XMLSerializerItems item = new XMLSerializerItems();
            item.items.Add(new Items("Item01", "Default_Item_01", "items_16", new List<string>(), new List<float>()));
            item.Save(Application.streamingAssetsPath + "/Modding/Items/GeneralItems/GeneralItems/GeneralItems.xml");

            #endregion
        }

        if (advancedSettings["Create Unit Example"])
        {
            #region Pokemon Example
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Pokemon/GeneralPokemon/GeneralPokemon");
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GeneralPokemon";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/Pokemon/GeneralPokemon/GeneralPokemon/Info.xml");
            }

            XMLSerializerPokemon pok = new XMLSerializerPokemon();
            pok.pokemon.Add(new Pokemon("Pokemon01", 5, 100, 50, new List<string>() { "Hearts" }, new List<float>() { 10.5332f }, new string[] {
                "Dark",
                "Bug"
            }, new string[]
            {
                "Default_0.1_Passive_GoldenHeart"
            }, new string[] {
                "Move01",
                "Move02"
            }, "pokemon_2"));
            pok.pokemon.Add(new Pokemon("Pokemon02", 10, 150, 80, new List<string>(), new List<float>(), new string[] {
                "Poison",
                "Bug"
            }, new string[]
            {
                "Default_0.1_Passive_GoldenHeart"
            }, new string[] {
                "Move01",
                "Move02"
            }, "pokemon_10"));
            pok.Save(Application.streamingAssetsPath + "/Modding/Pokemon/GeneralPokemon/GeneralPokemon/GeneralPokemon.xml");
        }
        #endregion

        if (advancedSettings["Create Type Example"])
        {
            #region Type Example
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Types/GeneralTypes/GeneralTypes");
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GeneralTypes";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/Types/GeneralTypes/GeneralTypes/Info.xml");
            }

            XMLSerializerTypes types = new XMLSerializerTypes();
            types.types.Add(new Types("Bug", "Bug", new string[] {
                "Bug"}, new string[] {
                "Poison"
            }, new string[] {
                "Poison"
            }));
            types.types.Add(new Types("Poison", "Poison",
                                        new string[] {
                "Bug"
            }, new string[] {
                "Bug"
            }, new string[]
            {
                "Poison"
            }
            ));
            types.types.Add(new Types("Dark", "Dark",
                                        new string[] {
                "Poison"
            }, new string[] {
                "Bug"
            }, new string[] {
            }
            ));
            types.Save(Application.streamingAssetsPath + "/Modding/Types/GeneralTypes/GeneralTypes/GeneralTypes.xml");
        }
        #endregion

        if (advancedSettings["Create Status Example"])
        {

            #region Status Example
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Statuses/GeneralStatuses/GeneralStatuses");
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GeneralStatuses";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/Statuses/GeneralStatuses/GeneralStatuses/Info.xml");
            }

            XMLSerializerStatuses statuses = new XMLSerializerStatuses();
            statuses.statuses.Add(new Status("Burned", "Default_0.1_Burned", "Default_0.1_Burned_Function", true, true, 2, Status.StatusType.beforeTurn, true));
            statuses.statuses.Add(new Status("Golden Heart", "Default_0.1_Passive_GoldenHeart", "Default_0.1_Passive_GoldenHeart_Function", false, false, 2, Status.StatusType.beforeEnemyAttack, false));

            statuses.Save(Application.streamingAssetsPath + "/Modding/Statuses/GeneralStatuses/GeneralStatuses/GeneralStatuses.xml");

            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/BurntAI");
            XMLSerializerFunction funcBurned = new XMLSerializerFunction();
            funcBurned.function = new Function("Default_0.1_Burned_Function", new FunctionCalls[] { FunctionCalls.none }, null, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.lookup, "Pokemon_HP", Values.currentPokemon, Values.pokemonHitPoints),
                    new Value (signs.minusEqual, "^^ - 3", Values.getPreviousValue, Values.numberValue, 3),
                    new Value (signs.exists, "Pokemon-Stat exists", Values.dictString, Values.NONE, 0, "If true go to 1 else go to 2")
                }, 1, 2),
                new Argument ("ArgumentTest_02BurntTrue", new Value[] {
                    new Value (signs.add, "Add 1 then return it", Values.dictString, Values.numberValue, 1, "burnt_i"),
                    new Value (signs.commandRun, "Randomise between 0 and 5 if it hits your number or lower then you 'win'", Values.randomiseEngine, Values.numberValue, 5),
                    new Value (signs.lessThen, "Your random number is less than i", Values.getPreviousValue, Values.dictString, 0, "burnt_i")
                }, 3, -1),
                new Argument ("ArgumentTest_03BurntFalse", new Value[] {
                    new Value (signs.commandRun, "Store i", Values.numberValue, Values.storeKeyString, 0, "burnt_i"),
                    new Value (signs.commandRun, "Randomise between 0 and 5 if it hits your number or lower then you 'win'", Values.randomiseEngine, Values.numberValue, 5),
                    new Value (signs.lessThen, "Your random number is less than i", Values.getPreviousValue, Values.dictString, 0, "burnt_i")
                }, 3, -1),
                new Argument ("ArugmentTest_04RemoveBurn", new Value[] {
                    new Value (signs.commandRun, "Remove Burn", Values.println, Values.stringValue, 0, "PokemonRecoveredFromBurn"),
                    new Value (signs.lookup, "Actually Remove Burn", Values.currentPokemon, Values.statusRemoveString, 0, "Default_0.1_Burned")
                }, -1, -1)
            }, "/(%TurnPokemon%)/ is burnt", "");
            funcBurned.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/BurntAI/BurntFunction.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Default_0.1_Burned_Function";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_Burnt";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/BurntAI/Info.xml");
            }

            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/GoldenHeartAI");
            XMLSerializerFunction funcGoldenHeart = new XMLSerializerFunction();
            funcGoldenHeart.function = new Function("Default_0.1_Passive_GoldenHeart", new FunctionCalls[] { FunctionCalls.none }, null, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.lookup, "Pokemon_MaxHP", Values.currentPokemon, Values.pokemonMaxHitPoints),
                    new Value (signs.minusEqual, "^^-10", Values.getPreviousValue, Values.numberValue, 10, ""),
                    new Value (signs.equals, "Exit Command", Values.numberValue, Values.numberValue, 0, "")
                }, -1, -1)
            }, "/(%!TurnPokemon%)/ took 10 damage", "");
            funcGoldenHeart.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/GoldenHeartAI/GoldenHeartAI.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Default_0.1_Passive_GoldenHeart";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GoldenHeart";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/GoldenHeartAI/Info.xml");
            }
        }
        #endregion

        if (advancedSettings["Create Move Example"])
        {

            #region Move Example
            {
                Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/Moves/GeneralMoves/GeneralMoves");
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_GeneralMoves";
                modInfo.versionNumber = 1;
                modInfo.Save(Path.Combine(Application.streamingAssetsPath, "Modding/Moves/GeneralMoves/GeneralMoves/Info.xml"));
            }

            XMLSerializerMoves moves = new XMLSerializerMoves();
            moves.moves.Add(new Move("Bug", "Move01", Battler.Battlers.enemy, "Default_Move01", new List<string>(), new List<float>()));
            moves.moves.Add(new Move("Poison", "Move02", Battler.Battlers.enemy, "Default_Move02", new List<string>(), new List<float>()));
            moves.Save(Application.streamingAssetsPath + "/Modding/Moves/GeneralMoves/GeneralMoves/GeneralMoves.xml");

            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove01");
            XMLSerializerFunction funcMove01 = new XMLSerializerFunction();
            funcMove01.function = new Function("FunctionMove01", new FunctionCalls[] { FunctionCalls.none }, null, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.equals, "Current Battler is enemy", Values.currentTurn, Values.numberValue, 1, "")
                }, 1, 2),
                new Argument ("Current Pokemon Is Enemy", new Value[] {
                    new Value (signs.lookup, "Get Pok HP", Values.currentAllyPokemon, Values.pokemonHitPoints),
                    new Value (signs.minusEqual, "^^ - 15", Values.getPreviousValue, Values.numberValue, 15),
                    new Value (signs.equals, "Exit Command", Values.numberValue, Values.numberValue, 0)
                }, -1, -1),
                new Argument ("Current Pokemon Is Ally", new Value[] {
                    new Value (signs.lookup, "Get Pok HP", Values.currentEnemyPokemon, Values.pokemonHitPoints),
                    new Value (signs.minusEqual, "^^ - 15", Values.getPreviousValue, Values.numberValue, 15),
                    new Value (signs.equals, "Exit Command", Values.numberValue, Values.numberValue, 0)
                }, -1, -1)
            }, "The /(%Turn%)/'s /(%TurnPokemon%)/ attacked dealing 15 damage", "");
            funcMove01.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove01/Move01.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Default_Move01";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_Move01";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove01/Info.xml");
            }

            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove02");
            XMLSerializerFunction funcMove02 = new XMLSerializerFunction();
            funcMove02.function = new Function("FunctionMove02", new FunctionCalls[] { FunctionCalls.none }, null, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.equals, "Current Battler is enemy", Values.currentTurn, Values.numberValue, 1, "")
                }, 1, 2),
                new Argument ("Current Pokemon Is Enemy", new Value[] {
                    new Value (signs.lookup, "Get Pok HP", Values.currentAllyPokemon, Values.pokemonHitPoints),
                    new Value (signs.minusEqual, "^^ - 5", Values.getPreviousValue, Values.numberValue, 5),
                    new Value (signs.lookup, "Pokemon_AddStatus", Values.currentAllyPokemon, Values.statusAddString, 0, "Default_0.1_Burned"),
                    new Value (signs.equals, "Exit Command", Values.numberValue, Values.numberValue, 0)
                }, -1, -1),
                new Argument ("Current Pokemon Is Ally", new Value[] {
                    new Value (signs.lookup, "Get Pok HP", Values.currentEnemyPokemon, Values.pokemonHitPoints),
                    new Value (signs.minusEqual, "^^ - 5", Values.getPreviousValue, Values.numberValue, 5),
                    new Value (signs.lookup, "Pokemon_AddStatus", Values.currentEnemyPokemon, Values.statusAddString, 0, "Default_0.1_Burned"),
                    new Value (signs.equals, "Exit Command", Values.numberValue, Values.numberValue, 0)
                }, -1, -1) }, "The /(%Turn%)/'s /(%TurnPokemon%)/ attacked dealing 5 damage + burn", "");
            funcMove02.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove02/Move02.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Default_Move02";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_Move02";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionMove02/Info.xml");
            }
        }
        #endregion

        if (advancedSettings["Create Function Example"])
        {

            #region Function Example
            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionAttackChoices");
            XMLSerializerFunction func = new XMLSerializerFunction();
            func.function = new Function("Function_Choose_Attack", new FunctionCalls[] { FunctionCalls.battleActionN }, new List<int> { 0, 1 }, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.commandRun, "Randomise 0 - 1", Values.randomiseEngine, Values.numberValue, 1),
                    new Value (signs.equals, "Old Value ^^ == 1", Values.getPreviousValue, Values.numberValue, 1)
                }, -2, -1) }, "", "");
            func.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionAttackChoices/AttackChoices.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Function_Action_Attack_Choices";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_ChooseMove";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionAttackChoices/Info.xml");
            }

            Directory.CreateDirectory(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionChooseAction");
            XMLSerializerFunction func2 = new XMLSerializerFunction();
            func2.function = new Function("Function_Choose_Action", new FunctionCalls[] { FunctionCalls.battlerChooseAttack }, null, new Argument[] { new Argument ("ArgumentTest_01", new Value[] {
                    new Value (signs.commandRun, "Randomise 0", Values.randomiseEngine, Values.numberValue, 0),
                    new Value (signs.equals, "Equal 0", Values.getPreviousValue, Values.numberValue, 0)
                }, -1, -1) }, "", "");
            func2.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionChooseAction/ChooseAction.xml");
            {
                XMLSerializer modInfo = new XMLSerializer();
                modInfo.modRunnable = "Function_Choose_Action";
                modInfo.modActive = true;
                modInfo.modName = "Default_Game_ChooseAction";
                modInfo.versionNumber = 1;
                modInfo.Save(Application.streamingAssetsPath + "/Modding/AI/GeneralAI/FunctionChooseAction/Info.xml");
            }
            #endregion
        }
        AssetDatabase.Refresh();
    }
}