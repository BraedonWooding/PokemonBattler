using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class SetupChoices : MonoBehaviour
{
    //Final Pokemon Choices
    public List<Pokemon> allyPokemonFinal = new List<Pokemon>();
    public List<Pokemon> enemyPokemonFinal = new List<Pokemon>();
    public List<Items> itemsFinal = new List<Items>();
    public List<Move> movesFinal = new List<Move>();
    public List<Sprite> trainerFinal = new List<Sprite>();

    public Startup startup;

    [SerializeField]
    private Dropdown[]
        allyPokemonDropdowns;

    [SerializeField]
    private Dropdown[]
        enemyPokemonDropdowns;

    [SerializeField]
    private Dropdown[]
        itemDropdowns;

    [SerializeField]
    private Dropdown[]
        trainerDropdowns;

    [SerializeField]
    private ModHandler
    modhandler;

    // Use this for initialization
    void Start()
    {
        startup = FindObjectOfType<Startup>();
        modhandler = FindObjectOfType<ModHandler>();

        SetupChoice();
    }

    public void RunSimulation()
    {
        //Retrieve the values and put them into a new array
        foreach (Dropdown drop in itemDropdowns)
        {
            if (startup.items.ContainsKey(drop.captionText.text))
                itemsFinal.Add(startup.items[drop.captionText.text]);
        }

        foreach (Dropdown drop in allyPokemonDropdowns)
        {
            if (startup.pokemon.ContainsKey(drop.captionText.text))
                allyPokemonFinal.Add(startup.pokemon[drop.captionText.text]);
        }

        foreach (Dropdown drop in enemyPokemonDropdowns)
        {
            if (startup.pokemon.ContainsKey(drop.captionText.text))
                enemyPokemonFinal.Add(startup.pokemon[drop.captionText.text]);
        }

        foreach (Dropdown drop in trainerDropdowns)
        {
            if (startup.trainerSprites.ContainsValue(drop.captionImage.sprite))
                trainerFinal.Add(drop.captionImage.sprite);
        }

        DontDestroyOnLoad(gameObject);
        Application.LoadLevel("Battler");
    }

    void OnLevelWasLoaded()
    {
        if (Application.loadedLevelName == "Battler")
        {
            //Level Loaded now input the data
            Battler battler = GameObject.Find("_SCRIPTS_BATTLER_").GetComponent<Battler>();

            Battler.allyPokemon = allyPokemonFinal;
            Battler.enemyPokemon = enemyPokemonFinal;

            battler.allyTrainerSprite = trainerFinal[0];
            battler.enemyTrainerSprite = trainerFinal[1];

            Battler.statuses = startup.statuses;
            Battler.types = startup.types;

            battler.startGame = true;
        }
    }

    public void SetupChoice()
    {
        List<XMLSerializer> xml = modhandler.GetItemXMLEnumeration();
        for (int p = 0; p < xml.Count; p++)
        {
            if (xml[p].modActive)
            {
                List<Items> im = modhandler.GetMod<List<Items>>(xml[p]);
                for (int i = 0; i < im.Count; i++)
                {
                    if (!startup.items.ContainsKey(im[i].iName))
                    {
                        startup.items.Add(im[i].iName, im[i]);
                        startup.items.ElementAt(startup.items.Count - 1).Value.iSprite = startup.itemSprites[startup.items.ElementAt(startup.items.Count - 1).Value.iSprite_Name];
                    }
                }
            }
        }

        xml = modhandler.GetTypeXMLEnumeration();
        for (int p = 0; p < xml.Count; p++)
        {
            if (xml[p].modActive)
            {
                List<Types> tv = modhandler.GetMod<List<Types>>(xml[p]);
                for (int i = 0; i < tv.Count; i++)
                {
                    if (!startup.types.ContainsKey(tv[i].tName))
                    {
                        startup.types.Add(tv[i].tID, tv[i]);
                    }
                }
            }
        }

        xml = modhandler.GetStatusXMLEnumeration();
        for (int p = 0; p < xml.Count; p++)
        {
            if (xml[p].modActive)
            {
                List<Status> mv = modhandler.GetMod<List<Status>>(xml[p]);
                for (int i = 0; i < mv.Count; i++)
                {
                    if (!startup.statuses.ContainsKey(mv[i].sName))
                    {
                        startup.statuses.Add(mv[i].sID, mv[i]);
                    }
                }
            }
        }

        xml = modhandler.GetMoveXMLEnumeration();
        for (int p = 0; p < xml.Count; p++)
        {
            if (xml[p].modActive)
            {
                List<Move> mv = modhandler.GetMod<List<Move>>(xml[p]);
                for (int i = 0; i < mv.Count; i++)
                {
                    if (!startup.moves.ContainsKey(mv[i].mName))
                    {
                        startup.moves.Add(mv[i].mName, mv[i]);
                        startup.moves.ElementAt(startup.moves.Count - 1).Value.mType = startup.types[startup.moves.ElementAt(startup.moves.Count - 1).Value.mType_Name];
                    }
                }
            }
        }

        xml = modhandler.GetPokemonXMLEnumeration();
        for (int p = 0; p < xml.Count; p++)
        {
            if (xml[p].modActive)
            {
                List<Pokemon> pk = modhandler.GetMod<List<Pokemon>>(xml[p]);
                for (int i = 0; i < pk.Count; i++)
                {
                    if (!startup.pokemon.ContainsKey(pk[i].pName))
                    {
                        startup.pokemon.Add(pk[i].pName, pk[i]);
                        startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pSprite = startup.pokemonSprites[startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pSprite_Name];

                        for (int j = 0; j < startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pStatus_Names.Count; j++)
                        {
                            startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pStatus.Add(startup.statuses[startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pStatus_Names[j]]);
                        }

                        for (int j = 0; j < startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pMoves_Names.Count; j++)
                        {
                            startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pMoves.Add(startup.moves[startup.pokemon.ElementAt(startup.pokemon.Count - 1).Value.pMoves_Names[j]]);
                        }
                    }
                }
            }
        }

        foreach (Dropdown dropDown in allyPokemonDropdowns)
        {
            dropDown.options.Add(new Dropdown.OptionData("Choose a pokemon"));
            foreach (Pokemon pok in startup.pokemon.Values)
                dropDown.options.Add(new Dropdown.OptionData(pok.pName, (startup.pokemonSprites.ContainsKey(pok.pSprite_Name) ? startup.pokemonSprites[pok.pSprite_Name] : null)));
        }

        foreach (Dropdown dropDown in enemyPokemonDropdowns)
        {
            dropDown.options.Add(new Dropdown.OptionData("Choose a pokemon"));
            foreach (Pokemon pok in startup.pokemon.Values)
                dropDown.options.Add(new Dropdown.OptionData(pok.pName, (startup.pokemonSprites.ContainsKey(pok.pSprite_Name) ? startup.pokemonSprites[pok.pSprite_Name] : null)));
        }

        for (int i = 0; i < trainerDropdowns.Length; i++)
        {
            foreach (Sprite trainer in startup.trainerSprites.Values)
                trainerDropdowns[i].options.Add(new Dropdown.OptionData(trainer));
        }

        foreach (Dropdown dropDown in itemDropdowns)
        {
            dropDown.options.Add(new Dropdown.OptionData("Choose a item"));
            foreach (Items item in startup.items.Values)
                dropDown.options.Add(new Dropdown.OptionData(item.iName, (startup.itemSprites.ContainsKey(item.iSprite_Name) ? startup.itemSprites[item.iSprite_Name] : null)));
        }
    }
}