using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Text;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using EnumLibrary;

public class Startup : MonoBehaviour
{
    private string pathCreation = "/BattlerData";

    public float versionNumber;
    bool forceUpdate = false;

    public Dictionary<string, Pokemon> pokemon = new Dictionary<string, Pokemon>();
    public Dictionary<string, Items> items = new Dictionary<string, Items>();
    public Dictionary<string, Move> moves = new Dictionary<string, Move>();
    public Dictionary<string, Sprite> pokemonSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> trainerSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Sprite> itemSprites = new Dictionary<string, Sprite>();
    public Dictionary<string, Types> types = new Dictionary<string, Types>();
    public Dictionary<string, Status> statuses = new Dictionary<string, Status>();

    [SerializeField]
    private ModHandler
        modhandler;

    void Awake()
    {
        versionNumber = PlayerPrefs.GetFloat("Version_Number");

        if (!Directory.Exists(Application.dataPath + pathCreation))
        {
            Directory.CreateDirectory(Application.dataPath + pathCreation);
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Saves");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Trainers");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/PokemonSprites");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Items");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/ItemSprites");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/AI");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Settings");
            Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario");
        }

        GameXMLSerializer file = GameXMLSerializer.Load(Application.streamingAssetsPath + "/Settings/Settings.xml");

        //Check version number if not same then delete the files under datapath normally
        //In test it will just recreate them
        if (file.versionNumber != versionNumber || forceUpdate)
        {
            File.Copy(Application.streamingAssetsPath + "/Settings/Settings.xml", Application.dataPath + pathCreation + "/Settings/Settings.xml", true);

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI"))
            {
                string[] directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI");

                for (int i = 0; i < directories.Length; i++)
                {
                    Directory.Delete(directories[i], true);
                }
            }

            File.Copy(Application.streamingAssetsPath + "/Settings/Settings.xml", Application.dataPath + pathCreation + "/Settings/Settings.xml", true);
            versionNumber = file.versionNumber;
            PlayerPrefs.SetFloat("Version_Number", versionNumber);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadAICore(); //Done
        LoadModdingData();  //Done
    }

    void OnLevelWasLoaded()
    {
        if (Application.loadedLevelName == "Starting")
        {
            SetupChoices setupChoices = FindObjectOfType<SetupChoices>();

            setupChoices.startup = this;
        }
    }

    public void LoadAICore()
    {
        if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI"))
        {
            string[] directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/AI/GeneralAI", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI/" + Path.GetFileName(d)))
                {
                    Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI/" + Path.GetFileName(d));

                    string[] files = Directory.GetFiles(d, "*.*");

                    // Copy the files and overwrite destination files if they already exist. 
                    foreach (string s in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        string fileName = Path.GetFileName(s);
                        string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI/" + Path.GetFileName(d), fileName);
                        File.Copy(s, destFile, true);
                    }
                }
            }
        }
        gameObject.GetComponent<AISystem>().RunSetup(Application.dataPath + pathCreation + "/Modding/AI/GeneralAI");
    }

    public void LoadModdingData()
    {
        if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Pokemon"))
        {
            #region LoadPokemonSprites

            Sprite[] pokemonSpritesLocal = Resources.LoadAll<Sprite>("Pokemon/Sprites");

            pokemonSprites = pokemonSpritesLocal.ToDictionary(p => p.name);

            FileInfo[] info = new DirectoryInfo(Application.dataPath + pathCreation + "/Modding/PokemonSprites").GetFiles("*.png");

            for (int i = 0; i < info.Length; i++)
            {
                byte[] data = File.ReadAllBytes(info[i].DirectoryName);
                Texture2D texture = new Texture2D(80, 80, TextureFormat.ARGB32, false);
                texture.LoadImage(data);
                texture.name = Path.GetFileNameWithoutExtension(info[i].FullName);
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, 80, 80), new Vector2(0.5f, 0.5f));
                pokemonSprites.Add(texture.name, newSprite);
            }

            #endregion

            #region LoadItemSprites

            Sprite[] itemSpritesLocal = Resources.LoadAll<Sprite>("Items/Sprites");

            itemSprites = itemSpritesLocal.ToDictionary(p => p.name);

            info = new DirectoryInfo(Application.dataPath + pathCreation + "/Modding/ItemSprites").GetFiles("*.png");

            for (int i = 0; i < info.Length; i++)
            {
                byte[] data = File.ReadAllBytes(info[i].DirectoryName);
                Texture2D texture = new Texture2D(32, 32, TextureFormat.ARGB32, false);
                texture.LoadImage(data);
                texture.name = Path.GetFileNameWithoutExtension(info[i].FullName);
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, 32, 32), new Vector2(0.5f, 0.5f));
                itemSprites.Add(texture.name, newSprite);
            }

            #endregion

            #region LoadTrainerSprites

            Sprite[] trainerSpritesLocal = Resources.LoadAll<Sprite>("Trainers");

            trainerSprites = trainerSpritesLocal.ToDictionary(t => t.name, v => v);

            info = new DirectoryInfo(Application.dataPath + pathCreation + "/Modding/Trainers").GetFiles("*.png");

            for (int i = 0; i < info.Length; i++)
            {
                byte[] data = File.ReadAllBytes(info[i].DirectoryName);
                Texture2D texture = new Texture2D(80, 80, TextureFormat.ARGB32, false);
                texture.LoadImage(data);
                texture.name = Path.GetFileNameWithoutExtension(info[i].FullName);
                Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, 80, 80), new Vector2(0.5f, 0.5f));
                trainerSprites.Add(texture.name, newSprite);
            }

            #endregion

            #region LoadStatuses

            string[] directories;

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Statuses/GeneralStatuses", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Statuses/GeneralStatuses/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Statuses", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerStatuses xmlStatuses = XMLSerializerStatuses.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlStatuses.statuses);
                            }
                        }
                    }
                }
            }

            #endregion

            #region LoadTypes

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Types/GeneralTypes", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Types/GeneralTypes/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Types", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerTypes xmlTypes = XMLSerializerTypes.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlTypes.types);
                            }
                        }
                    }
                }
            }

            #endregion

            #region LoadScenario

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Scenario/GeneralScenario", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Scenario/GeneralScenario/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Scenario", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerScenarios xmlScenario = XMLSerializerScenarios.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlScenario.scenario);
                            }
                        }
                    }
                }
            }

            #endregion

            #region LoadMoves

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Moves/GeneralMoves", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Moves/GeneralMoves/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Moves", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerMoves xmlMoves = XMLSerializerMoves.Load(mods[j].DirectoryName + "/" + mods[j].Name);


                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlMoves.moves);
                            }
                        }
                    }
                }
            }

            #endregion

            #region LoadPokemon

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Pokemon/GeneralPokemon", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Pokemon/GeneralPokemon/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Pokemon", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerPokemon xmlPokemon = XMLSerializerPokemon.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlPokemon.pokemon);
                            }
                        }
                    }
                }
            }

            #endregion

            #region LoadItems

            if (Directory.Exists(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems"))
            {
                directories = Directory.GetDirectories(Application.streamingAssetsPath + "/Modding/Items/GeneralItems", "*", SearchOption.AllDirectories);

                foreach (string d in directories)
                {
                    if (!Directory.Exists(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems/" + Path.GetFileName(d)))
                    {
                        Directory.CreateDirectory(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems/" + Path.GetFileName(d));

                        string[] files = Directory.GetFiles(d, "*.*");

                        // Copy the files and overwrite destination files if they already exist. 
                        foreach (string s in files)
                        {
                            // Use static Path methods to extract only the file name from the path.
                            string fileName = Path.GetFileName(s);
                            string destFile = Path.Combine(Application.dataPath + pathCreation + "/Modding/Items/GeneralItems/" + Path.GetFileName(d), fileName);
                            File.Copy(s, destFile, true);
                        }
                    }
                }
            }

            directories = Directory.GetDirectories(Application.dataPath + pathCreation + "/Modding/Items", "*", SearchOption.AllDirectories);

            foreach (string d in directories)
            {
                FileInfo[] mods = new DirectoryInfo(d).GetFiles("*.xml", SearchOption.AllDirectories);
                for (int j = 0; j < mods.Length; j++)
                {
                    if (File.Exists(d + "/Info.xml"))
                    {
                        XMLSerializer modInfo = XMLSerializer.Load(d + "/Info.xml");
                        modInfo.path = d + "/Info.xml";
                        modInfo.Save(d + "/Info.xml");

                        if (modInfo.modName != "")
                        {
                            if (!mods[j].Name.Contains("Info"))
                            {
                                XMLSerializerItems xmlItems = XMLSerializerItems.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                                if (modInfo != null)
                                    modhandler.AddMod(modInfo, xmlItems.items);
                            }
                        }
                    }
                }
            }
            #endregion
        }
    }
}