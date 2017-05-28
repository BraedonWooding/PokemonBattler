using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using EnumLibrary;
using System.ComponentModel;

public class ModHandler : MonoBehaviour
{
    Dictionary<XMLSerializer, Function> allFunctionMods = new Dictionary<XMLSerializer, Function>();
    Dictionary<XMLSerializer, List<Pokemon>> allPokemonMods = new Dictionary<XMLSerializer, List<Pokemon>>();
    Dictionary<XMLSerializer, List<Items>> allItemMods = new Dictionary<XMLSerializer, List<Items>>();
    Dictionary<XMLSerializer, List<Move>> allMoveMods = new Dictionary<XMLSerializer, List<Move>>();
    Dictionary<XMLSerializer, List<Types>> allTypeMods = new Dictionary<XMLSerializer, List<Types>>();
    Dictionary<XMLSerializer, Scenario> allScenarioMods = new Dictionary<XMLSerializer, Scenario>();
    Dictionary<XMLSerializer, List<Status>> allStatusMods = new Dictionary<XMLSerializer, List<Status>>();

    Dictionary<string, Function> functionModFinder = new Dictionary<string, Function>();
    Dictionary<string, List<Pokemon>> pokemonModFinder = new Dictionary<string, List<Pokemon>>();
    Dictionary<string, List<Items>> itemModFinder = new Dictionary<string, List<Items>>();
    Dictionary<string, List<Move>> moveModFinder = new Dictionary<string, List<Move>>();
    Dictionary<string, List<Types>> typeModFinder = new Dictionary<string, List<Types>>();
    Dictionary<string, Scenario> scenarioModFinder = new Dictionary<string, Scenario>();
    Dictionary<string, List<Status>> statusModFinder = new Dictionary<string, List<Status>>();

    private GameObject prefabModImg;
    [SerializeField]
    private GameObject
        modHolder;

    public void AddMod(XMLSerializer modInfo, Function func)
    {
        allFunctionMods.Add(modInfo, func);
        functionModFinder.Add(modInfo.modName, func);
    }

    public void AddMod(XMLSerializer modInfo, List<Status> status)
    {
        allStatusMods.Add(modInfo, status);
        statusModFinder.Add(modInfo.modName, status);
    }

    public void AddMod(XMLSerializer modInfo, Scenario scenario)
    {
        allScenarioMods.Add(modInfo, scenario);
        scenarioModFinder.Add(modInfo.modName, scenario);
    }

    public void AddMod(XMLSerializer modInfo, List<Types> type)
    {
        allTypeMods.Add(modInfo, type);
        typeModFinder.Add(modInfo.modName, type);
    }

    public void AddMod(XMLSerializer modInfo, List<Pokemon> pokemon)
    {
        allPokemonMods.Add(modInfo, pokemon);
        pokemonModFinder.Add(modInfo.modName, pokemon);
    }

    public void AddMod(XMLSerializer modInfo, List<Items> items)
    {
        allItemMods.Add(modInfo, items);
        itemModFinder.Add(modInfo.modName, items);
    }
    public void AddMod(XMLSerializer modInfo, List<Move> moves)
    {
        allMoveMods.Add(modInfo, moves);
        moveModFinder.Add(modInfo.modName, moves);
    }

    public T GetMod<T>(XMLSerializer modInfo = null, string modName = "")
    {
        T t = default(T);
        if (typeof(List<Pokemon>).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allPokemonMods[modInfo];
        }
        else if (typeof(Function).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allFunctionMods[modInfo];
        }
        else if (typeof(List<Move>).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allMoveMods[modInfo];
        }
        else if (typeof(List<Status>).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allStatusMods[modInfo];
        }
        else if (typeof(List<Items>).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allItemMods[modInfo];
        }
        else if (typeof(Scenario).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allScenarioMods[modInfo];
        }
        else if (typeof(List<Types>).IsAssignableFrom(typeof(T)))
        {
            t = (T)(object)allTypeMods[modInfo];
        }
        else
            t = default(T);

        return t;
    }

    public List<Function> GetFunctionModEnumeration()
    {
        return allFunctionMods.Values.ToList();
    }

    public List<Scenario> GetScenarioModEnumeration()
    {
        return allScenarioMods.Values.ToList();
    }

    public List<List<Pokemon>> GetPokemonModEnumeration()
    {
        return allPokemonMods.Values.ToList();
    }

    public List<List<Status>> GetStatusModEnumeration()
    {
        return allStatusMods.Values.ToList();
    }

    public List<List<Move>> GetMoveModEnumeration()
    {
        return allMoveMods.Values.ToList();
    }

    public List<List<Items>> GetItemModEnumeration()
    {
        return allItemMods.Values.ToList();
    }

    public List<List<Types>> GetTypeModEnumeration()
    {
        return allTypeMods.Values.ToList();
    }

    public List<XMLSerializer> GetFunctionXMLEnumeration()
    {
        return allFunctionMods.Keys.ToList();
    }

    public List<XMLSerializer> GetScenarioXMLEnumeration()
    {
        return allScenarioMods.Keys.ToList();
    }

    public List<XMLSerializer> GetTypeXMLEnumeration()
    {
        return allTypeMods.Keys.ToList();
    }

    public List<XMLSerializer> GetStatusXMLEnumeration()
    {
        return allStatusMods.Keys.ToList();
    }

    public List<XMLSerializer> GetPokemonXMLEnumeration()
    {
        return allPokemonMods.Keys.ToList();
    }

    public List<XMLSerializer> GetMoveXMLEnumeration()
    {
        return allMoveMods.Keys.ToList();
    }

    public List<XMLSerializer> GetItemXMLEnumeration()
    {
        return allItemMods.Keys.ToList();
    }

    public List<string> GetFunctionModNames()
    {
        return functionModFinder.Keys.ToList();
    }

    public List<string> GetTypeModNames()
    {
        return typeModFinder.Keys.ToList();
    }

    public List<string> GetStatusModNames()
    {
        return statusModFinder.Keys.ToList();
    }

    public List<string> GetPokemonModNames()
    {
        return pokemonModFinder.Keys.ToList();
    }

    public List<string> GetItemModNames()
    {
        return itemModFinder.Keys.ToList();
    }

    public List<string> GetScenarioModNames()
    {
        return scenarioModFinder.Keys.ToList();
    }

    public List<string> GetMoveModNames()
    {
        return moveModFinder.Keys.ToList();
    }

    public void ChangeValue<T>(int elementI, bool newValue)
    {
        if (typeof(Function).IsAssignableFrom(typeof(T)))
        {
            allFunctionMods.ElementAt(elementI).Key.modActive = newValue;
            allFunctionMods.ElementAt(elementI).Key.Save(allFunctionMods.ElementAt(elementI).Key.path);

            functionModFinder.Clear();
            foreach (var info in allFunctionMods)
                functionModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Types).IsAssignableFrom(typeof(T)))
        {
            allTypeMods.ElementAt(elementI).Key.modActive = newValue;
            allTypeMods.ElementAt(elementI).Key.Save(allTypeMods.ElementAt(elementI).Key.path);

            typeModFinder.Clear();
            foreach (var info in allTypeMods)
                typeModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Pokemon).IsAssignableFrom(typeof(T)))
        {
            allPokemonMods.ElementAt(elementI).Key.modActive = newValue;
            allPokemonMods.ElementAt(elementI).Key.Save(allPokemonMods.ElementAt(elementI).Key.path);

            pokemonModFinder.Clear();
            foreach (var info in allPokemonMods)
                pokemonModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Move).IsAssignableFrom(typeof(T)))
        {
            allMoveMods.ElementAt(elementI).Key.modActive = newValue;
            allMoveMods.ElementAt(elementI).Key.Save(allMoveMods.ElementAt(elementI).Key.path);

            moveModFinder.Clear();
            foreach (var info in allMoveMods)
                moveModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Items).IsAssignableFrom(typeof(T)))
        {
            allItemMods.ElementAt(elementI).Key.modActive = newValue;
            allItemMods.ElementAt(elementI).Key.Save(allItemMods.ElementAt(elementI).Key.path);

            itemModFinder.Clear();
            foreach (var info in allItemMods)
                itemModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Scenario).IsAssignableFrom(typeof(T)))
        {
            allScenarioMods.ElementAt(elementI).Key.modActive = newValue;
            allScenarioMods.ElementAt(elementI).Key.Save(allScenarioMods.ElementAt(elementI).Key.path);

            scenarioModFinder.Clear();
            foreach (var info in allScenarioMods)
                scenarioModFinder.Add(info.Key.modName, info.Value);
        }
        else if (typeof(Status).IsAssignableFrom(typeof(T)))
        {
            allStatusMods.ElementAt(elementI).Key.modActive = newValue;
            allStatusMods.ElementAt(elementI).Key.Save(allStatusMods.ElementAt(elementI).Key.path);

            statusModFinder.Clear();
            foreach (var info in allStatusMods)
                statusModFinder.Add(info.Key.modName, info.Value);
        }
    }

    public void ModActive()
    {
        prefabModImg = Resources.Load<GameObject>("PrefabModImage");

        foreach (Transform child in modHolder.transform)
            Destroy(child.gameObject);
        //Doing function mods
        for (int i = 0; i < allFunctionMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allFunctionMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Function";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allFunctionMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allFunctionMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing scenario mods
        for (int i = 0; i < allScenarioMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allScenarioMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Scenario";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allScenarioMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allScenarioMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing type mods
        for (int i = 0; i < allTypeMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allTypeMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Type";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allTypeMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allTypeMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing pokemon mods
        for (int i = 0; i < allPokemonMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allPokemonMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Pokemon";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allPokemonMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allPokemonMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing status mods
        for (int i = 0; i < allStatusMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allStatusMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Status";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allStatusMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allStatusMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing item mods
        for (int i = 0; i < allItemMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allItemMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Item";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allItemMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allItemMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }

        //Doing move mods
        for (int i = 0; i < allMoveMods.Count; i++)
        {
            GameObject go = Instantiate(prefabModImg);
            go.GetComponentInChildren<Toggle>().isOn = allMoveMods.ElementAt(i).Key.modActive;
            go.GetComponentInChildren<Toggle>().tag = "Move";
            Text[] txt = go.GetComponentsInChildren<Text>();
            txt[0].text = allMoveMods.ElementAt(i).Key.modName;
            txt[1].text = "Version: " + allMoveMods.ElementAt(i).Key.versionNumber;
            go.name = "" + i;
            go.transform.SetParent(modHolder.transform);
        }
    }
}