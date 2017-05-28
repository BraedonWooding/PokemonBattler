using UnityEngine;
using EnumLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class ActionButton : MonoBehaviour
{
    public string aName;
    public int id;
    public Choices currentChoice;
    public Battler battler = null;

    public enum Choices
    {
        attack,
        bag,
        pokemon,
        run
    }

    public void ClickedActionButton(bool human) //bool for ai access
    {
        if (battler == null)
            battler = GameObject.Find("_SCRIPTS_").GetComponent<Battler>();

        battler.ClearText();

        Pokemon pok;
        Pokemon enemyPok;

        if (human)
        {
            pok = Battler.allyPokemon[Battler.currentAllyPokemonID];
            enemyPok = Battler.enemyPokemon[Battler.currentEnemyPokemonID];
        }
        else
        {
            pok = Battler.enemyPokemon[Battler.currentEnemyPokemonID];
            enemyPok = Battler.allyPokemon[Battler.currentAllyPokemonID];
        }

        List<Status> allyList = pok.pStatus.OrderBy(o => o.sPriority).ToList();
        List<Status> enemyList = enemyPok.pStatus.OrderBy(o => o.sPriority).ToList();

        //Sanity Check
        if (pok.pName == "" || enemyPok.pName == "")
        {
            Debug.Log("Error Pokemon / EnemyPokemon is null");
            Battler.currentTurn = (Battler.currentTurn == Battler.Battlers.ally) ? Battler.Battlers.enemy : Battler.Battlers.ally;
            Battler.battlerPanel.SetActive(true);
            return;
        }

        //Do Effects

        switch (currentChoice)
        {

            case Choices.attack:
                if (pok.pMoves.Count > id)
                {
                    //Before Attacking
                    foreach (Status status in allyList)
                    {
                        if (status.sType == Status.StatusType.beforeAttacking || status.sType == Status.StatusType.beforeAnyAttack)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                            }
                        }
                    }

                    if (enemyPok.pStatus.Count > 0)
                    {
                        foreach (Status status in enemyList)
                        {
                            if (status.sType == Status.StatusType.afterEnemyAttack || status.sType == Status.StatusType.afterAnyAttack)
                            {
                                try
                                {
                                    battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                                }
                                catch (MyException ex)
                                {
                                    Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                    Debug.Log(ex.Message);
#endif
                                }
                            }
                        }
                    }

                    try
                    {
                        battler.aiCore.Run(pok.pMoves[id].mFunctionName, null, pok.pMoves[id]);
                    }
                    catch (MyException ex)
                    {
                        Battler.textToDisplay.Enqueue("Something went wrong with this move, choose another");
#if UNITY_EDITOR
                        Debug.Log(ex.Message);
#endif
                        return;
                    }
                }
                else
                {
                    Battler.textToDisplay.Enqueue("Choose an action!");
                    return;
                }

                foreach (Status status in allyList)
                {
                    if (status.sType == Status.StatusType.afterAttacking || status.sType == Status.StatusType.afterAnyAttack)
                    {
                        try
                        {
                            battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                        }
                        catch (MyException ex)
                        {
                            Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                            Debug.Log(ex.Message);
#endif
                        }
                    }
                }

                if (enemyPok.pStatus.Count > 0)
                {
                    foreach (Status status in enemyList)
                    {
                        if (status.sType == Status.StatusType.afterEnemyAttack || status.sType == Status.StatusType.afterAnyAttack)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                            }
                        }
                    }
                }

                break;

            case Choices.bag:
                if (Battler.items.Count > id)
                {
                    foreach (Status status in allyList)
                    {
                        if (status.sType == Status.StatusType.beforeItem || status.sType == Status.StatusType.afterAnyItem)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                            }
                        }
                    }

                    if (enemyPok.pStatus.Count > 0)
                    {
                        foreach (Status status in enemyList)
                        {
                            if (status.sType == Status.StatusType.beforeEnemyItem || status.sType == Status.StatusType.beforeAnyItem)
                            {
                                try
                                {
                                    battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                                }
                                catch (MyException ex)
                                {
                                    Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                    Debug.Log(ex.Message);
#endif
                                }
                            }
                        }
                    }

                    try
                    {
                        battler.aiCore.Run(Battler.items[id].iFunction_Name, null, null, Battler.items[id]);
                    }
                    catch (MyException ex)
                    {
                        Battler.textToDisplay.Enqueue("Something went wrong with this move, choose another");
#if UNITY_EDITOR
                        Debug.Log(ex.Message);
#endif
                        return;
                    }

                    foreach (Status status in allyList)
                    {
                        if (status.sType == Status.StatusType.afterItem || status.sType == Status.StatusType.afterAnyItem)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                                return;
                            }
                        }
                    }

                    if (enemyPok.pStatus.Count > 0)
                    {
                        foreach (Status status in enemyList)
                        {
                            if (status.sType == Status.StatusType.afterEnemyItem || status.sType == Status.StatusType.afterAnyItem)
                            {
                                try
                                {
                                    battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                                }
                                catch (MyException ex)
                                {
                                    Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                    Debug.Log(ex.Message);
#endif
                                    return;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Battler.textToDisplay.Enqueue("Choose an action!");
                    return;
                }
                break;

            case Choices.pokemon:
                //Set Current Pokemon
                if (pok.pStatus.Count > 0)
                {
                    if (allyList.ElementAt(0).sPriority == -1)
                    {
                        Battler.textToDisplay.Enqueue(pok.pName + " Is " + allyList.ElementAt(0).sName);
                        return;
                    }
                }

                foreach (Status status in allyList)
                {
                    if (status.sType == Status.StatusType.beforeSwitch || status.sType == Status.StatusType.beforeAnySwitch)
                    {
                        try
                        {
                            battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                        }
                        catch (MyException ex)
                        {
                            Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                            Debug.Log(ex.Message);
#endif
                        }
                    }
                }

                if (enemyPok.pStatus.Count > 0)
                {
                    foreach (Status status in enemyList)
                    {
                        if (status.sType == Status.StatusType.beforeEnemySwitch || status.sType == Status.StatusType.beforeAnySwitch)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                            }
                        }
                    }
                }

                if (human)
                    Battler.currentAllyPokemonID = id;
                else
                    Battler.currentEnemyPokemonID = id;

                foreach (Status status in allyList)
                {
                    if (status.sType == Status.StatusType.afterSwitch || status.sType == Status.StatusType.afterAnySwitch)
                    {
                        try
                        {
                            battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                        }
                        catch (MyException ex)
                        {
                            Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                            Debug.Log(ex.Message);
#endif
                        }
                    }
                }

                if (enemyPok.pStatus.Count > 0)
                {
                    foreach (Status status in enemyList)
                    {
                        if (status.sType == Status.StatusType.afterEnemySwitch || status.sType == Status.StatusType.afterAnySwitch)
                        {
                            try
                            {
                                battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                            }
                            catch (MyException ex)
                            {
                                Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
#if UNITY_EDITOR
                                Debug.Log(ex.Message);
#endif
                            }
                        }
                    }
                }
                break;

            case Choices.run:
                //Error since value shouldn't end up as run?
                Debug.Log("Error Run Value Undefined");
                Battler.textToDisplay.Enqueue("Choose an action!");
                return;
        }
        Battler.battlerPanel.SetActive(true);

        StartCoroutine(Wait(enemyPok, pok, enemyList, allyList));
    }

    IEnumerator Wait(Pokemon enemyPok, Pokemon pok, List<Status> enemyList, List<Status> allyList)
    {
        yield return 0;

        while (AISystem.running || Battler.running)
        {
            yield return 0;
        }

        foreach (Status status in allyList)
        {
            if (status.sType == Status.StatusType.afterTurn || status.sType == Status.StatusType.afterAnyTurn)
            {
                try
                {
                    battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                }
                catch (MyException ex)
                {
                    Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
                    ex.ToString();
                }
            }
        }

        if (enemyPok.pStatus.Count > 0)
        {
            foreach (Status status in enemyList)
            {
                if (status.sType == Status.StatusType.afterEnemyTurn || status.sType == Status.StatusType.afterAnyTurn)
                {
                    try
                    {
                        battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                    }
                    catch (MyException ex)
                    {
                        Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
                        ex.ToString();
                    }
                }
            }
        }

        Battler.currentTurn = (Battler.currentTurn == Battler.Battlers.ally) ? Battler.Battlers.enemy : Battler.Battlers.ally;
        Battler.battlerPanel.SetActive(true);

        foreach (Status status in allyList)
        {
            if (status.sType == Status.StatusType.beforeEnemyTurn || status.sType == Status.StatusType.beforeAnyTurn)
            {
                try
                {
                    battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                }
                catch (MyException ex)
                {
                    Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
                    ex.ToString();
                }
            }
        }

        if (enemyPok.pStatus.Count > 0)
        {
            foreach (Status status in enemyList)
            {
                if (status.sType == Status.StatusType.beforeTurn || status.sType == Status.StatusType.beforeAnyTurn)
                {
                    try
                    {
                        battler.aiCore.Run(status.sFunctionName, null, null, null, null, status);
                    }
                    catch (MyException ex)
                    {
                        Battler.textToDisplay.Enqueue("Something went wrong with " + status.sName + " status effect, continuing");
                        ex.ToString();
                    }
                }
            }
        }

        yield break;
    }
}