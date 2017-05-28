using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using EnumLibrary;
using System.ComponentModel;

[XmlRoot("Mod_Settings")]
public class XMLSerializer
{
    [XmlAttribute("Mod_Runnable")]
    public string
        modRunnable;
    [XmlAttribute("Version_Number")]
    public float
        versionNumber;
    [XmlAttribute("Mod_Name")]
    public string
        modName;
    [XmlAttribute("Mod_Active")]
    public bool
        modActive;
    [XmlIgnore()]
    public string
        path;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializer));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializer));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializer;
    }
}

[XmlRoot("Game_Settings")]
public class GameXMLSerializer
{
    [XmlAttribute("Version_Number")]
    public float
        versionNumber;
    [XmlAttribute("Game_Name")]
    public string
        gameName;
    [XmlAttribute("Game_Volume")]
    public float volume;
    [XmlAttribute("Game_VolumeOn")]
    public bool volumeOn;
    [XmlAttribute("Game_VolumeSettings")]
    public MusicManager.Settings settings;
    [XmlIgnore()]
    public string
        path;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(GameXMLSerializer));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static GameXMLSerializer Load(string path)
    {
        var serializer = new XmlSerializer(typeof(GameXMLSerializer));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as GameXMLSerializer;
    }
}

[XmlRoot("Collection_Of_Pokemon")]
public class XMLSerializerPokemon
{
    [XmlArray("Pokemon"), XmlArrayItem("Pokemon")]
    public List<Pokemon>
        pokemon = new List<Pokemon>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerPokemon));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerPokemon Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerPokemon));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerPokemon;
    }
}

[XmlRoot("Collection_Of_Items")]
public class XMLSerializerItems
{
    [XmlArray("Items"), XmlArrayItem("Item")]
    public List<Items>
        items = new List<Items>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerItems));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerItems Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerItems));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerItems;
    }
}

[XmlRoot("Collection_Of_Moves")]
public class XMLSerializerMoves
{
    [XmlArray("Moves"), XmlArrayItem("Move")]
    public List<Move>
        moves = new List<Move>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerMoves));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerMoves Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerMoves));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerMoves;
    }
}

[XmlRoot("Collection_Of_Types")]
public class XMLSerializerTypes
{
    [XmlArray("Types"), XmlArrayItem("Type")]
    public List<Types>
        types = new List<Types>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerTypes));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerTypes Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerTypes));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerTypes;
    }
}

[XmlRoot("Collection_Of_Statuses")]
public class XMLSerializerStatuses
{
    [XmlArray("Statuses"), XmlArrayItem("Status")]
    public List<Status>
        statuses = new List<Status>();

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerStatuses));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerStatuses Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerStatuses));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerStatuses;
    }
}

[XmlRoot("Collection_Of_")]
public class XMLSerializerScenarios
{
    [XmlElement("Scenario")]
    public Scenario scenario;

    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerScenarios));
        using (var stream = new FileStream(path, FileMode.Create))
            serializer.Serialize(stream, this);
    }

    public static XMLSerializerScenarios Load(string path)
    {
        var serializer = new XmlSerializer(typeof(XMLSerializerScenarios));
        using (var stream = new FileStream(path, FileMode.Open))
            return serializer.Deserialize(stream) as XMLSerializerScenarios;
    }
}

[Serializable]
public class Scenario
{
    [XmlAttribute("Scenario_Name")]
    public string sName;
    [XmlAttribute("User_Select_Units")]
    public bool sSelectUnits;
    [XmlAttribute("Scenario_HasChildren")] //Uses more than a battle
    public bool sPathed;

    [XmlAttribute("Scenario_Credits")]
    public string sCredits;

    [XmlElement("Scenario_Title_Color")]
    public Color32 sTitleColor;

    [XmlArray("Scenario_Finishes"), XmlArrayItem("Scenario_Finish")]
    public List<ScenarioFinishAction> sOnFinish;

    [XmlArray("Scenario_Nodes"), XmlArrayItem("Scenario_Node")]
    public List<ScenarioNode> sNodes;

    public Scenario()
    {
        sNodes = new List<ScenarioNode>();
        sOnFinish = new List<ScenarioFinishAction>();
    }

    public Scenario(string name, bool selectUnits, bool pathed, string credits, ScenarioFinishAction[] onFinish, ScenarioNode[] nodes, Color32 titleColor)
    {
        sName = name;
        sSelectUnits = selectUnits;
        sPathed = pathed;
        sCredits = credits;

        sOnFinish = onFinish.ToList();
        sNodes = nodes.ToList();

        sTitleColor = titleColor;
    }
}

[Serializable]
public class ScenarioNode
{
    [XmlAttribute("Node_Name")]
    public string nName;
    [XmlAttribute("Node_HasChildren")]
    public bool nPathed;
    [XmlAttribute("Node_HasButtons")]
    public bool nHasButtons;
    [XmlArray("Node_Buttons"), XmlArrayItem("Node_Button")]
    public List<ScenarioNodeButton> nButtons;
    [XmlArray("Node_Children_Names"), XmlArrayItem("Node_Children_Name")]
    public List<string> nChildrenNames;
    [XmlAttribute("Node_Title")]
    public string nTitle;
    [XmlElement("Node_Title_Color")]
    public Color32 nTitleColor;
    [XmlAttribute("Node_Message")]
    public string nMessage;
    [XmlElement("Node_Message_Color")]
    public Color32 nMessageColor;

    public ScenarioNode()
    {
        nButtons = new List<ScenarioNodeButton>();
        nChildrenNames = new List<string>();
    }

    public ScenarioNode(string name, bool pathed, bool hasButtons, ScenarioNodeButton[] buttons, string[] childrenNames, string title, Color32 titleColor, string message, Color32 messageColor)
    {
        nName = name;
        nPathed = pathed;
        nHasButtons = hasButtons;
        nButtons = buttons.ToList();
        nChildrenNames = childrenNames.ToList();
        nTitle = title;
        nTitleColor = titleColor;
        nMessage = message;
        nMessageColor = messageColor;
    }
}

[Serializable]
public class ScenarioNodeButton
{
    [XmlAttribute("Button_Name")]
    public string bName;
    [XmlAttribute("Button_Title")]
    public string bTitle;
    [XmlAttribute("Button_Action_NodeString")]
    public string bString;
    [XmlElement("Button_Action_Battle")]
    public ScenarioBattle bBattle;
    [XmlAttribute("Button_Action_NodeX")]
    public int bInt;
    [XmlElement("Button_Color")]
    public Color32 bColor;

    [XmlAttribute("Button_Action")]
    public ScenarioButtonAction bAction;

    [XmlIgnore]
    public bool bBattleSpecified
    {
        get
        {
            return bBattle.bIsOn;
        }
    }

    public ScenarioNodeButton()
    {

    }

    public ScenarioNodeButton(string name, string title, Color32 color, ScenarioButtonAction action, int Int = 0, string String = "", ScenarioBattle battle = null)
    {
        bName = name;
        bTitle = title;
        bColor = color;
        bInt = Int;
        bString = String;
        bAction = action;
        bBattle = battle;
    }
}

[Serializable]
public class ScenarioBattle
{
    [XmlArray("Ally_Names")]
    public List<string> sbAllyNames;
    [XmlArray("Enemy_Names")]
    public List<string> sbEnemyNames;

    [XmlAttribute("Battle_IsOn")]
    public bool bIsOn = false;

    [XmlAttribute("Button_WinAction")]
    public ScenarioButtonAction sbWinAction;
    [XmlAttribute("Button_WinAction_NodeString")]
    public string sbWinString;
    [XmlAttribute("Button_WinAction_NodeX")]
    public int sbWinInt;

    [XmlAttribute("Button_LossAction")]
    public ScenarioButtonAction sbLossAction;
    [XmlAttribute("Button_LossAction_NodeString")]
    public string sbLossString;
    [XmlAttribute("Button_LossAction_NodeX")]
    public int sbLossInt;

    public ScenarioBattle()
    {
        sbAllyNames = new List<string>();
        sbEnemyNames = new List<string>();
        bIsOn = true;
    }

    public ScenarioBattle(string[] allyNames, string[] enemyNames, ScenarioButtonAction winAction, string winString, int winInt, ScenarioButtonAction lossAction, string lossString, int lossInt)
    {
        sbAllyNames = allyNames.ToList();
        sbEnemyNames = enemyNames.ToList();

        sbWinAction = winAction;
        sbWinInt = winInt;
        sbWinString = winString;

        sbLossAction = lossAction;
        sbLossInt = lossInt;
        sbLossString = lossString;

        bIsOn = true;
    }
}

[Serializable]
public class Status
{
    public enum StatusType
    {
        beforeAttacking,
        afterAttacking,
        beforeEnemyAttack,
        afterEnemyAttack,
        beforeAnyAttack,
        afterAnyAttack,
        beforeItem,
        afterItem,
        beforeEnemyItem,
        afterEnemyItem,
        beforeAnyItem,
        afterAnyItem,
        beforeSwitch,
        afterSwitch,
        beforeEnemySwitch,
        afterEnemySwitch,
        beforeAnySwitch,
        afterAnySwitch,
        beforeTurn,
        afterTurn,
        beforeEnemyTurn,
        afterEnemyTurn,
        beforeAnyTurn,
        afterAnyTurn,
        beforeRun,
        beforeEnemyRun,
        beforeAnyRun
    }

    [XmlAttribute("Status_Name")]
    public string
        sName;
    [XmlAttribute("Status_ID")]
    public string
        sID;
    [XmlAttribute("Status_Function_Name")]
    public string
        sFunctionName;
    [XmlAttribute("Status_Priority")]
    public int
        sPriority;
    [XmlAttribute("Status_Display")]
    public bool
        sDisplay;
    [XmlAttribute("Status_Removable")]
    public bool
        sRemovable;
    [XmlAttribute("Status_Activate")]
    public StatusType
        sType;
    [XmlAttribute("Status_Carry_Over")]
    public bool
        sCarryOver = false;

    public Status(string name, string id, string functionName, bool display, bool removable, int priority, StatusType type, bool carryOver)
    {
        sName = name;
        sID = id;
        sFunctionName = functionName;
        sDisplay = display;
        sRemovable = removable;
        sPriority = priority;
        sType = type;
        sCarryOver = carryOver;
    }

    public Status()
    {
    }
}

[Serializable]
public class Types
{
    [XmlAttribute("Type_Name")]
    public string
        tName;
    [XmlAttribute("Type_ID")]
    public string
        tID;
    [XmlArray("Type_Weaknesses"), XmlArrayItem("Type_Weakness")]
    public List<string>
        tWeaknessTable;
    [XmlArray("Type_Resistances"), XmlArrayItem("Type_Resistance")]
    public List<string>
        tResistTable;
    [XmlArray("Type_Immunities"), XmlArrayItem("Type_Immunity")]
    public List<string>
        tImmunityTable;

    public Types(string id, string name, string[] weaknesses, string[] resistances, string[] immunities)
    {
        tID = id;
        tName = name;
        if (weaknesses != null)
            tWeaknessTable = weaknesses.ToList();
        else
            tWeaknessTable = new List<string>();
        if (resistances != null)
            tResistTable = resistances.ToList();
        else
            tResistTable = new List<string>();
        if (immunities != null)
            tImmunityTable = immunities.ToList();
        else
            tImmunityTable = new List<string>();
    }

    public Types()
    {
        tWeaknessTable = new List<string>();
        tResistTable = new List<string>();
        tImmunityTable = new List<string>();
    }
}

[Serializable]
public class Exposable
{
    public string identifier;
    public float value;

    public Exposable(string id, float val)
    {
        identifier = id;
        value = val;
    }

    public Exposable()
    {
    }
}

[Serializable]
public class Pokemon
{
    [XmlAttribute("Pokemon_Name")]
    public string
        pName;
    [XmlAttribute("Pokemon_Sprite_Name")]
    public string
        pSprite_Name;
    [XmlArray("Pokemon_Types"), XmlArrayItem("Pokemon_Type")]
    public List<string>
        pTypes;
    [XmlArray("Pokemon_Move_Names"), XmlArrayItem("Pokemon_Move_Name")]
    public List<string>
        pMoves_Names;
    [XmlIgnore()]
    public List<Move>
        pMoves;
    [XmlAttribute("Pokemon_Speed")]
    public int
        pSpeed;
    [XmlAttribute("Pokemon_MaxHP")]
    public int
        pMaxHp;
    [XmlAttribute("Pokemon_HP")]
    public int
        pHp;
    [XmlArray("Pokemon_Status_Names"), XmlArrayItem("Pokemon_Status_Name")]
    public List<string>
        pStatus_Names;
    [XmlIgnore()]
    public List<Status>
        pStatus;
    [XmlIgnore()]
    public Sprite
        pSprite;
    [XmlArray("Pokemon_Exposable"), XmlArrayItem("Pokemon_Exposable")]
    public List<Exposable>
        pExposable;

    public Pokemon(string name, int speed, int maxHp, int hp, List<string> exposableName, List<float> exposableValue, string[] types = null, string[] status = null, string[] movesNames = null, string sprite_Name = "")
    {
        if (types == null)
            pTypes = new List<string>();
        else
            pTypes = types.ToList();
        if (movesNames == null)
            pMoves_Names = new List<string>();
        else
            pMoves_Names = movesNames.ToList();
        if (status == null)
            pStatus = new List<Status>();
        else
            pStatus_Names = status.ToList();

        pName = name;
        pSpeed = speed;
        pMaxHp = maxHp;
        pHp = hp;
        pSprite_Name = sprite_Name;

        pExposable = new List<Exposable>();

        for (int i = 0; i < Mathf.Min(exposableName.Count, exposableValue.Count); i++)
        {
            pExposable.Add(new Exposable(exposableName[i], exposableValue[i]));
        }
    }

    public Pokemon()
    {
        pTypes = new List<string>();
        pMoves = new List<Move>();
        pMoves_Names = new List<string>();
        pStatus_Names = new List<string>();
        pStatus = new List<Status>();
        pExposable = new List<Exposable>();
    }
}

[Serializable]
public class Move
{
    [XmlAttribute("Move_Name")]
    public string
        mName;
    [XmlAttribute("Move_Target")]
    public Battler.Battlers
        mTarget;
    [XmlAttribute("Move_Type_Name")]
    public string
        mType_Name;
    [XmlIgnore()]
    public Types
        mType;
    [XmlIgnore()]
    public Function
        mFunction;
    [XmlAttribute("Move_Function_Name")]
    public string
        mFunctionName;
    [XmlArray("Move_Exposables"), XmlArrayItem("Move_Exposable")]
    public List<Exposable>
        mExposable;

    public Move(string type, string name, Battler.Battlers target, string function_Name, List<string> exposableName, List<float> exposableValue)
    {
        mTarget = target;
        mType_Name = type;
        mName = name;
        mFunctionName = function_Name;
        mExposable = new List<Exposable>();

        for (int i = 0; i < Mathf.Min(exposableName.Count, exposableValue.Count); i++)
        {
            mExposable.Add(new Exposable(exposableName[i], exposableValue[i]));
        }
    }

    public Move()
    {
        mExposable = new List<Exposable>();
    }
}

[Serializable]
public class Items
{
    [XmlAttribute("Item_Name")]
    public string
        iName;
    [XmlAttribute("Item_Sprite_Name")]
    public string
        iSprite_Name;
    [XmlIgnore()]
    public Sprite
        iSprite;
    [XmlAttribute("Item_Function_Name")]
    public string
        iFunction_Name;
    [XmlIgnore()]
    public Function
        iFunction;
    [XmlArray("Item_Exposables"), XmlArrayItem("Item_Exposable")]
    public List<Exposable>
        iExposable;

    public Items(string name, string function_Name, string sprite_Name, List<string> exposableName, List<float> exposableValue)
    {
        iName = name;
        iSprite_Name = sprite_Name;
        iFunction_Name = function_Name;

        iExposable = new List<Exposable>();

        for (int i = 0; i < Mathf.Min(exposableName.Count, exposableValue.Count); i++)
        {
            iExposable.Add(new Exposable(exposableName[i], exposableValue[i]));
        }
    }

    public Items()
    {
        iExposable = new List<Exposable>();
    }
}