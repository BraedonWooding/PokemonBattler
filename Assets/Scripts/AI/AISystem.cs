using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;
using System.IO;
using EnumLibrary;
using System.ComponentModel;
using System.Reflection;

public class AISystem : MonoBehaviour
{
    public ModHandler modhandler;
    public ModCompiler compiler;

    public static bool running;

    public void RunSetup(string path)
    {
        if (modhandler == null)
            modhandler = FindObjectOfType<ModHandler>();

        if (compiler == null)
            compiler = FindObjectOfType<ModCompiler>();

        string[] directories = Directory.GetDirectories(path, "*", SearchOption.AllDirectories);

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
                            XMLSerializerFunction function = XMLSerializerFunction.Load(mods[j].DirectoryName + "/" + mods[j].Name);
                            if (modInfo != null)
                                modhandler.AddMod(modInfo, function.function);
                        }
                    }
                }
            }
        }
    }

    public void Run(string functionName = "", Function function = null, Move currentMove = null, Items currentItem = null, Pokemon currentPokemon = null, Status currentStatus = null)
    {
        compiler = FindObjectOfType<ModCompiler>();

        Function finalFunc = function;
        if (functionName != "")
        {
            List<XMLSerializer> serializerKeys = modhandler.GetFunctionXMLEnumeration();

            for (int i = 0; i < serializerKeys.Count; i++)
            {
                if (serializerKeys[i].modRunnable == functionName)
                    if (serializerKeys[i].modActive)
                    {
                        //Run function
                        finalFunc = modhandler.GetMod<Function>(serializerKeys[i], "");
                    }
            }
        }

        if (finalFunc != null)
        {
            compiler.currentItem = currentItem;
            compiler.currentMove = currentMove;
            compiler.currentPokemon = currentPokemon;
            compiler.currentStatus = currentStatus;
            StartCoroutine(RunXML(finalFunc));
        }
        else
        {
            throw new MyException("Function Doesn't Exist");
        }
    }

    IEnumerator RunXML(Function func)
    {
        running = true;
        //Running
        if (func.fBeforeMessage != "")
        {
            Battler.textToDisplay.Clear();
            Battler.textToDisplay.Enqueue(func.fBeforeMessage);
            yield return 0;

            while (Battler.running)
            {
                Debug.Log("Waiting for you: " + Battler.currentTurn);
                yield return 0;
            }
        }

        compiler.ConvertXMLThenRun(func);
        running = false;
        yield break;
    }
}

[System.Serializable]
public class MyException : Exception
{
    public MyException()
    {
    }
    public MyException(string message) : base(message)
    {
    }
    public MyException(string message, Exception inner) : base(message, inner)
    {
    }
    protected MyException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }
}