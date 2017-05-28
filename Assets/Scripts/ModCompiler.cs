using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using EnumLibrary;
using System.ComponentModel;

[Serializable]
public class ModCompiler : MonoBehaviour
{
    [SerializeField]
    public List<MutableValue>
        values = new List<MutableValue>();
    [SerializeField]
    public Dictionary<string, float>
        storeValues = new Dictionary<string, float>();
    public int callerList = 0;
    [SerializeField]
    public Function
        currentFunction;
    [SerializeField]
    public List<Argument>
        argumentsCallerOrder = new List<Argument>();
    [SerializeField]
    public Pokemon
        pokValue;
    [SerializeField]
    public Move
        currentMove = null;
    [SerializeField]
    public Items
        currentItem = null;
    [SerializeField]
    public Pokemon
        currentPokemon = null;
    [SerializeField]
    public Status
        currentStatus = null;

    public Dictionary<Status, Dictionary<string, float>> carryOvers = new Dictionary<Status, Dictionary<string, float>>();


    private enum PokemonPossibleChangers
    {
        pokemonHP,
        pokemonSpeed,
        pokemonMaxHP,
        pokemonTypeN,
        pokemonStatusN,
        pokemonMoveN
    }

    private enum MovePossibleChangers
    {
        moveType,
        moveDmg,
        moveChance,
        moveTarget,
        moveStatusAddN,
        moveStatusRemoveN
    }

    private enum ValueTypes
    {
        [Description("Sign")]
        boolean,
        [Description(@"Sign|Values")]
        value,
        [Description("Sign")]
        valueChanger,
        [Description(@"Sign|Values")]
        specialValues,
        [Description("Values")]
        lookup,
        [Description("UNKNOWN")]
        unknown,
        [Description("Values")]
        command,
        [Description("Values")]
        retrievables,
        [Description("Values")]
        retrieveChangeables,
        [Description("Values")]
        dynamicConstants
    }

    //Convert the jibber jabber xml code format into a useable c# runnable
    public void ConvertXMLThenRun(Function function)
    {
        Debug.Log("Running" + function.fName);
        callerList = 0;
        values = new List<MutableValue>();
        currentFunction = function;
        if (currentStatus != null && currentStatus.sCarryOver && carryOvers.ContainsKey(currentStatus))
            storeValues = carryOvers[currentStatus];
        else
            storeValues = new Dictionary<string, float>();

        if (function.fArguments.Count > 0)
        {
            int i = 0;
            while (i >= 0)
            {
                values = new List<MutableValue>();
                i = RunArgument(currentFunction.fArguments[i]);
                Debug.Log(i);
            }

            if (currentStatus != null && currentStatus.sCarryOver)
            {
                if (carryOvers.ContainsKey(currentStatus))
                    carryOvers[currentStatus] = storeValues;
                else
                    carryOvers.Add(currentStatus, storeValues);
            }
        }

        //Finished Do rest
        StartCoroutine(StallGame(currentFunction));

    }

    IEnumerator StallGame(Function func)
    {
        //Running
        if (func.fAfterMessage != "")
        {
            Battler.textToDisplay.Clear();
            Battler.textToDisplay.Enqueue(func.fAfterMessage);
            yield return 0;

            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return 0;
            }
        }

        yield break;
    }

    public int RunArgument(Argument argument)
    {
        argument.aCallerID = callerList++;
        argumentsCallerOrder.Add(argument);

        for (int i = 0; i < argument.aValues.Count; i++)
        {

            ValueTypes value = GetValueType(argument.aValues[i].vSign);

            if (value == ValueTypes.boolean)
            {
                //Boolean end of statement
                //Get first value
                bool? boolean = CalculateValue(argument.aValues[i]);
                if (boolean != null && boolean.Value)
                {
                    if (argumentsCallerOrder[argumentsCallerOrder.Count - 1].aTrueID >= 0)
                    {
                        return argumentsCallerOrder[argumentsCallerOrder.Count - 1].aTrueID;
                    }
                    else
                    {
                        //Run Function Calling
                        //Run Function Calling
                        foreach (FunctionCalls fc in currentFunction.fCalls)
                        {
                            switch (fc)
                            {
                                case FunctionCalls.battleActionN:
                                    //	Debug.Log (currentFunction.fName + "/" + currentFunction.fParameters.Count);
                                    int buttonValue = currentFunction.fParameters[argumentsCallerOrder[argumentsCallerOrder.Count - 1].aFalseID + 1];
                                    Battler.buttons[buttonValue].ClickedActionButton(false);
                                    break;

                                case FunctionCalls.battlerChooseAttack:
                                    Battler.instance.ButtonPressed(Battler.Battlers.enemy, ActionButton.Choices.attack);
                                    break;

                                case FunctionCalls.none:
                                    break;
                            }
                        }
                        return argumentsCallerOrder[argumentsCallerOrder.Count - 1].aTrueID;
                    }
                }
                else if (boolean != null && !boolean.Value)
                {
                    if (argumentsCallerOrder[argumentsCallerOrder.Count - 1].aFalseID >= 0)
                    {
                        return argumentsCallerOrder[argumentsCallerOrder.Count - 1].aFalseID;
                    }
                    else
                    {
                        //Run Function Calling
                        foreach (FunctionCalls fc in currentFunction.fCalls)
                        {
                            switch (fc)
                            {
                                case FunctionCalls.battleActionN:
                                    //	Debug.Log (currentFunction.fName + "/" + currentFunction.fParameters.Count);
                                    int buttonValue = currentFunction.fParameters[argumentsCallerOrder[argumentsCallerOrder.Count - 1].aFalseID + 1];
                                    Battler.buttons[buttonValue].ClickedActionButton(false);
                                    break;

                                case FunctionCalls.battlerChooseAttack:
                                    Battler.instance.ButtonPressed(Battler.Battlers.enemy, ActionButton.Choices.attack);
                                    break;

                                case FunctionCalls.none:
                                    break;
                            }
                        }
                        return argumentsCallerOrder[argumentsCallerOrder.Count - 1].aFalseID;
                    }
                }
            }
            else if (value == ValueTypes.value)
            {
                //Value continue statement after this
                MutableValue returnedValue = CalculateValue(argument.aValues[i], value);
                if (values.Count == 0 || values[values.Count - 1] != returnedValue)
                {
                    values.Add(returnedValue);
                }

                if (argument.aValues[i].vValue1 == Values.dictStringStore)
                {
                    if (argument.aValues[i].vValueString != "")
                        storeValues[argument.aValues[i].vValueString] = (float)returnedValue.vValue;
                }
                else if (argument.aValues[i].vValue1 == Values.dictNameStore)
                {
                    if (argument.aValues[i].vName != "")
                        storeValues[argument.aValues[i].vName] = (float)returnedValue.vValue;
                }
            }
            else if (value == ValueTypes.valueChanger)
            {
                //Don't auto save value
                MutableValue newValue = CalculateValue(argument.aValues[i], value);
            }
            else if (value == ValueTypes.specialValues)
            {
                var returnedValue = CalculateValue(argument.aValues[i], value);
                if (argument.aValues[i].vValue1 == Values.randomiseEngine || argument.aValues[i].vSign == signs.lookup)
                {
                    values.Add(returnedValue);
                }
                //COMMENTED OUT SINCE MAKES NO SENSE IN CURRENT CONTEXT!!!
                //NONE, Run and other commands?  Double check the absolute value
                /*    if (values.Count == 0 || values[values.Count - 1] != returnedValue)
                    {
                        values.Add(returnedValue);
                    } */
            }
        }
        return 0;
    }

    private PokemonMutableValue GetValue(Value value, Argument arg)
    {
        pokValue = (GetValue(value, arg, false).GetValue() as Pokemon);

        return new PokemonMutableValue(pokValue, PokemonMutableValue.PokemonType.none);
    }

    private MutableValue GetValue(Value value, Argument arg, bool skipFirst = false)
    {
        if (!skipFirst)
        {
            switch (value.vValue1)
            {
                case Values.getPreviousValue:
                    if (values.Count > 0)
                        return values[values.Count - 1];
                    break;

                case Values.getPreviousValueOfN:
                    if (values.Count > value.vValueNumber)
                        return values[values.Count - (1 + (int)value.vValueNumber)];
                    break;

                case Values.dictString:
                case Values.dictStringStore:
                    if (storeValues.ContainsKey(value.vValueString))
                    {
                        return new MutableValue(storeValues[value.vValueString]);
                    }
                    break;

                case Values.dictName:
                case Values.dictNameStore:
                    if (storeValues.ContainsKey(value.vName))
                    {
                        return new MutableValue(storeValues[value.vName]);
                    }
                    break;

                case Values.pokemonElementString:
                    return new ExposableMutableValue(pokValue.pExposable.Find(x => x.identifier == value.vValueString));

                case Values.itemElementString:
                    return new ExposableMutableValue(currentItem.iExposable.Find(x => x.identifier == value.vValueString));

                case Values.moveElementString:
                    return new ExposableMutableValue(currentMove.mExposable.Find(x => x.identifier == value.vValueString));

                case Values.items:
                    if (Battler.items.Count > (int)value.vValueNumber)
                        return new ItemMutableValue(Battler.items[(int)value.vValueNumber]);
                    break;

                case Values.currentMove0:
                    if ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 0 : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 0)
                    {
                        Move m = ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[0] : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[0]);
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentMove1:
                    if ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 1 : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 1)
                    {
                        Move m = ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[1] : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[1]);
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentMove2:
                    if ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 2 : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 2)
                    {
                        Move m = ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[2] : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[2]);
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentMove3:
                    if ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 3 : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 3)
                    {
                        Move m = ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[3] : Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[3]);
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentAllyMove0:
                    if (Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 0)
                    {
                        Move m = Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[0];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentAllyMove1:
                    if (Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 1)
                    {
                        Move m = Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[1];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentAllyMove2:
                    if (Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 2)
                    {
                        Move m = Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[22];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentAllyMove3:
                    if (Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves.Count > 3)
                    {
                        Move m = Battler.allyPokemon[Battler.currentAllyPokemonID].pMoves[3];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentEnemyMove0:
                    if (Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 0)
                    {
                        Move m = Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[0];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentEnemyMove1:
                    if (Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 1)
                    {
                        Move m = Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[1];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentEnemyMove2:
                    if (Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 2)
                    {
                        Move m = Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[2];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.currentEnemyMove3:
                    if (Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves.Count > 3)
                    {
                        Move m = Battler.enemyPokemon[Battler.currentEnemyPokemonID].pMoves[3];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.allyPokemon:
                    if (Battler.allyPokemon.Count > (int)value.vValueNumber)
                        return new PokemonMutableValue(Battler.allyPokemon[(int)value.vValueNumber]);
                    break;

                case Values.enemyPokemon:
                    if (Battler.enemyPokemon.Count > (int)value.vValueNumber)
                        return new PokemonMutableValue(Battler.enemyPokemon[(int)value.vValueNumber]);
                    break;

                case Values.currentEnemyPokemon:
                    if (Battler.enemyPokemon.Count > Battler.currentEnemyPokemonID)
                    {
                        Pokemon p = Battler.enemyPokemon[Battler.currentEnemyPokemonID];
                        return new PokemonMutableValue(p);
                    }
                    break;

                case Values.currentAllyPokemon:
                    if (Battler.allyPokemon.Count > Battler.currentAllyPokemonID)
                    {
                        Pokemon p = Battler.allyPokemon[Battler.currentAllyPokemonID];
                        return new PokemonMutableValue(p);
                    }
                    break;

                case Values.currentPokemon:
                    if ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon.Count > Battler.currentAllyPokemonID : Battler.enemyPokemon.Count > Battler.currentEnemyPokemonID)
                    {
                        Pokemon p = ((Battler.currentTurn == Battler.Battlers.ally) ? Battler.allyPokemon[Battler.currentAllyPokemonID] : Battler.enemyPokemon[Battler.currentEnemyPokemonID]);
                        PokemonMutableValue pm = new PokemonMutableValue(p);
                        Debug.Log((pm.GetValue() as Pokemon).pHp);
                        return new PokemonMutableValue(p);
                    }
                    break;

                case Values.currentTurn:
                    Debug.Log(Battler.currentTurn);
                    if (Battler.currentTurn == Battler.Battlers.none)
                    {
                        return new MutableValue(null);
                    }
                    else
                        return new MutableValue((int)Battler.currentTurn);

                case Values.currentItemOpt:
                    if (currentItem != null)
                    {
                        return new ItemMutableValue(currentItem);
                    }
                    break;

                case Values.currentMoveOpt:
                    if (currentMove != null)
                    {
                        return new MoveMutableValue(currentMove);
                    }
                    break;

                case Values.currentPokemonOpt:
                    if (currentPokemon != null)
                    {
                        return new PokemonMutableValue(currentPokemon, PokemonMutableValue.PokemonType.none);
                    }
                    break;

                case Values.NONE:
                case Values.NULL:
                    return new MutableValue(null);

                case Values.numberValue:
                    return new MutableValue((float)value.vValueNumber);

                default:
                    return new MutableValue(null);

                    //COMMANDS NOT INCORPORATED HERE

            }
        }
        if (skipFirst)
        {
            pokValue = new Pokemon();
            if (value.vValue2 == Values.pokemonHitPoints || value.vValue2 == Values.pokemonMaxHitPoints || value.vValue2 == Values.pokemonSpeed || value.vValue2 == Values.pokemonMoveN || value.vValue2 == Values.pokemonStatusN || value.vValue2 == Values.pokemonTypeN)
            {
                pokValue = (GetValue(value, arg, false).GetValue() as Pokemon);
            }

            switch (value.vValue2)
            {

                case Values.currentTurn:
                    Debug.Log(Battler.currentTurn);
                    if (Battler.currentTurn == Battler.Battlers.none)
                    {
                        return new MutableValue(null);
                    }
                    else
                        return new MutableValue((int)Battler.currentTurn);


                case Values.getPreviousValue:
                    if (values.Count > 0)
                        return values[values.Count - 1];
                    break;

                case Values.getPreviousValueOfN:
                    if (values.Count > value.vValueNumber)
                        return values[values.Count - (1 + (int)value.vValueNumber)];
                    break;

                case Values.dictString:
                    if (storeValues.ContainsKey(value.vValueString))
                    {
                        return new MutableValue(storeValues[value.vValueString]);
                    }
                    break;

                case Values.dictName:
                    if (storeValues.ContainsKey(value.vName))
                    {
                        return new MutableValue(storeValues[value.vName]);
                    }
                    break;

                case Values.pokemonHitPoints:
                    return new PokemonMutableValue(pokValue, PokemonMutableValue.PokemonType.hp);

                case Values.pokemonSpeed:
                    return new PokemonMutableValue(pokValue, PokemonMutableValue.PokemonType.speed);

                case Values.pokemonMaxHitPoints:
                    return new PokemonMutableValue(pokValue, PokemonMutableValue.PokemonType.maxHP);

                case Values.pokemonStatusN:
                    if (pokValue.pStatus.Count > (int)value.vValueNumber)
                        return new StatusMutableValue(pokValue.pStatus[(int)value.vValueNumber]);
                    break;

                case Values.pokemonTypeN:
                    if (pokValue.pTypes.Count > (int)value.vValueNumber)
                        return new TypeMutableValue(Battler.GetType(pokValue.pTypes[(int)value.vValueNumber]));
                    break;

                case Values.pokemonMoveN:
                    if (pokValue.pMoves.Count > (int)value.vValueNumber)
                    {
                        Move m = pokValue.pMoves[(int)value.vValueNumber];
                        return new MoveMutableValue(m);
                    }
                    break;

                case Values.statusAddString:
                    if (!pokValue.pStatus.Contains(Battler.GetStatus(value.vValueString)))
                    {
                        Status stat = Battler.GetStatus(value.vValueString);
                        if (stat != null)
                            pokValue.pStatus.Add(stat);

                        if (pokValue.pName == Battler.allyPokemon[Battler.currentAllyPokemonID].pName)
                        {
                            //Current Pokemon is ally
                            Battler.allyPokemon[Battler.currentAllyPokemonID].pStatus.Add(stat);
                        }
                        else
                        {
                            Battler.enemyPokemon[Battler.currentEnemyPokemonID].pStatus.Add(stat);
                        }
                    }
                    break;

                case Values.statusRemoveString:
                    if (pokValue.pStatus.Contains(Battler.GetStatus(value.vValueString)))
                    {
                        Status stat = Battler.GetStatus(value.vValueString);
                        if (stat != null && stat.sRemovable)
                            pokValue.pStatus.Remove(stat);
                    }
                    break;

                case Values.statusRemoveAll:
                    return new MutableValue(null);

                case Values.pokemonElementString:
                    return new ExposableMutableValue(pokValue.pExposable.Find(x => x.identifier == value.vValueString));

                case Values.itemElementString:
                    return new ExposableMutableValue(currentItem.iExposable.Find(x => x.identifier == value.vValueString));

                case Values.moveElementString:
                    return new ExposableMutableValue(currentMove.mExposable.Find(x => x.identifier == value.vValueString));

                case Values.NONE:
                case Values.NULL:
                    return new MutableValue(null);

                case Values.numberValue:
                    return new MutableValue((float?)value.vValueNumber);

                default:
                    return new MutableValue(null);
                    //COMMANDS NOT INCORPORATED HERE
            }
        }
        return new MutableValue(null);
    }

    private bool? CalculateValue(Value value)
    {
        switch (value.vSign)
        {
            case signs.equals:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) == (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) != (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?))
                    return false;
                break;

            case signs.doesNotEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) == (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?))
                    return false;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) != (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?))
                    return true;
                break;

            case signs.doesNotExist:
                if (value.vValue1 == Values.dictString || value.vValue1 == Values.dictStringStore)
                {
                    if (storeValues.ContainsKey(value.vValueString))
                        return false;
                    else
                        return true;
                }
                else if (value.vValue1 == Values.dictName || value.vValue1 == Values.dictNameStore)
                {
                    if (storeValues.ContainsKey(value.vName))
                        return false;
                    else
                        return true;
                }
                else if (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() == null)
                    return true;
                else if (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() != null)
                    return false;
                break;

            case signs.exists:

                if (value.vValue1 == Values.dictString || value.vValue1 == Values.dictStringStore)
                {
                    if (storeValues.ContainsKey(value.vValueString))
                        return true;
                    else
                        return false;
                }
                else if (value.vValue1 == Values.dictName || value.vValue1 == Values.dictNameStore)
                {
                    if (storeValues.ContainsKey(value.vName))
                        return true;
                    else
                        return false;
                }
                else if (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() == null)
                    return false;
                else if (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() != null)
                    return true;
                break;

            case signs.greaterThen:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) > (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) <= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.greaterThenOrEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) >= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) < (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.lessThen:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) < (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) >= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.lessThenOrEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) <= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) > (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.percentageDoesNotEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 != (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 == (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.percentageGreater:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 > (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 <= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.percentageGreaterEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 >= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 < (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.percentageLess:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 < (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 >= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            case signs.percentageLessEqual:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 <= (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 > (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;
            case signs.percentageEqualTo:
                if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 == (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return true;
                else if ((GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * 100 != (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?))
                    return false;
                break;

            default:
                return false;
        }
        return false;
    }

    private MutableValue CalculateValue(Value value, ValueTypes type)
    {
        //Convert to a readable statement
        switch (type)
        {
            case ValueTypes.value:

                switch (value.vSign)
                {
                    case signs.add:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) + (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            return newMutableValue;
                        }
                    case signs.minus:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) - (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            return newMutableValue;
                        }
                    case signs.times:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) * (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            return newMutableValue;
                        }
                    case signs.divide:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) / (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            return newMutableValue;
                        }
                    case signs.root:
                        {
                            MutableValue newMutableValue = new MutableValue(Mathf.Pow((float)(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?), 1f / (float)(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?)));
                            return newMutableValue;
                        }

                    case signs.power:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = Mathf.Pow(((float)(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?)), ((float)(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?)));
                            return newMutableValue;
                        }
                    case signs.modulus:
                        {
                            MutableValue newMutableValue = new MutableValue(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            newMutableValue.vValue = (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?) % (GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?);
                            return newMutableValue;
                        }
                    default:
                        return new MutableValue(0);
                }

            case ValueTypes.valueChanger:
                PokemonMutableValue valuePok = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false) as PokemonMutableValue;
                switch (value.vSign)
                {
                    case signs.addEqual:
                        valuePok.SetValue((ObjectToFloat(valuePok.GetValue()) + ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue())));
                        return new MutableValue(valuePok.GetValue() as float?);
                    case signs.minusEqual:
                        valuePok.SetValue((ObjectToFloat(valuePok.GetValue()) - ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue())));
                        return new MutableValue(valuePok.GetValue() as float?);
                    case signs.timesEqual:
                        valuePok.SetValue((ObjectToFloat(valuePok.GetValue()) * ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue())));
                        return new MutableValue(valuePok.GetValue() as float?);
                    case signs.divideEqual:
                        valuePok.SetValue((ObjectToFloat(valuePok.GetValue()) / ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue())));
                        return new MutableValue(valuePok.GetValue() as float?);
                    case signs.rootEqual:
                        {
                            MutableValue secondValue = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true);
                            valuePok.SetValue(Mathf.Pow((valuePok.GetValue() != null) ? ObjectToFloat(valuePok.GetValue()) : 0, (secondValue.GetValue() != null) ? 1f / ObjectToFloat(secondValue.GetValue()) : 0));
                            return new MutableValue(valuePok.GetValue() as float?);
                        }
                    case signs.powerEqual:
                        {
                            MutableValue secondValue = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true);
                            valuePok.SetValue(Mathf.Pow((valuePok.GetValue() != null) ? ObjectToFloat(valuePok.GetValue()) : 0, (secondValue.GetValue() != null) ? ObjectToFloat(secondValue.GetValue()) : 0));
                            return new MutableValue(valuePok.GetValue() as float?);
                        }
                    case signs.modulusEqual:
                        valuePok.SetValue((ObjectToFloat(valuePok.GetValue()) % ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue())));
                        return new MutableValue(valuePok.GetValue() as float?);
                    case signs.percentageEqual:
                        valuePok.SetValue(((ObjectToFloat(valuePok.GetValue()) / ObjectToFloat(GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue()))) * 100);
                        return new MutableValue(valuePok.GetValue() as float?);
                    default:
                        break;
                }
                break;
            case ValueTypes.specialValues:
                switch (value.vSign)
                {
                    case signs.lookup:
                        var valueFirst = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true);
                        return valueFirst;
                    case signs.commandRun:
                        //Execute Run Command based of first function
                        if (GetValueType(value.vValue1) == ValueTypes.command)
                            switch (value.vValue1)
                            {
                                case Values.clearData:
                                    values.Clear();
                                    break;

                                case Values.println:
                                    Println(value.vValueString);
                                    break;

                                case Values.clearKeys:
                                    storeValues.Clear();
                                    break;

                                case Values.clearKeyString:
                                    storeValues.Remove(value.vValueString);
                                    break;

                                case Values.randomiseEngine:
                                    float? v2 = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], true).GetValue() as float?;
                                    if (v2 != null)
                                    {
                                        if (v2 > 0)
                                        {
                                            float randomResult = UnityEngine.Random.Range(0, (float)v2);
                                            return new MutableValue(randomResult);
                                        }
                                        else if (v2 < 0)
                                        {
                                            float randomResult = UnityEngine.Random.Range((float)v2, 0);
                                            return new MutableValue(randomResult);
                                        }
                                        else
                                        {
                                            return new MutableValue(0);
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                        if (GetValueType(value.vValue2) == ValueTypes.command)
                            switch (value.vValue2)
                            {
                                case Values.storeData:
                                    MutableValue valueGet = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false);
                                    values.Add(valueGet);
                                    break;

                                case Values.storeKeyString:
                                    if (!storeValues.ContainsKey(value.vValueString))
                                    {
                                        float? ex = GetValue(value, argumentsCallerOrder[argumentsCallerOrder.Count - 1], false).GetValue() as float?;
                                        storeValues.Add((value.vValueString != "") ? value.vValueString : value.vName, (ex != null) ? (float)ex : 0);
                                    }
                                    break;

                                case Values.storePokElementString:
                                    if (!pokValue.pExposable.Exists(x => x.identifier == value.vValueString))
                                    {
                                        pokValue.pExposable.Add(new Exposable(value.vValueString, (float)value.vValueNumber));
                                    }
                                    break;

                                case Values.storeItemElementString:
                                    if (!currentItem.iExposable.Exists(x => x.identifier == value.vValueString))
                                    {
                                        currentItem.iExposable.Add(new Exposable(value.vValueString, (float)value.vValueNumber));
                                    }
                                    break;

                                case Values.storeMoveElementString:
                                    if (!currentMove.mExposable.Exists(x => x.identifier == value.vValueString))
                                    {
                                        currentMove.mExposable.Add(new Exposable(value.vValueString, (float)value.vValueNumber));
                                    }
                                    break;

                                case Values.runCommand:
                                    //Doesn't need anything special just a empty space
                                    break;
                            }
                        break;

                    case signs.NONE:
                    default:
                        break;
                }
                break;

            default:
                return new MutableValue(0);
        }
        return new MutableValue(0);
    }

    static ValueTypes GetValueType(signs sign)
    {
        if (sign.GetHashCode() < 50)
            return ValueTypes.boolean;
        else if (sign.GetHashCode() > 49 && sign.GetHashCode() < 100)
            return ValueTypes.value;
        else if (sign.GetHashCode() > 99 && sign.GetHashCode() < 150)
            return ValueTypes.valueChanger;
        else if (sign.GetHashCode() > 149)
            return ValueTypes.specialValues;
        else
            return ValueTypes.unknown;
    }

    static ValueTypes GetValueType(Values value)
    {
        if (value.GetHashCode() < 50)
            return ValueTypes.lookup;
        else if (value.GetHashCode() > 49 && value.GetHashCode() < 100)
            return ValueTypes.retrievables;
        else if (value.GetHashCode() > 99 && value.GetHashCode() < 150)
            return ValueTypes.retrieveChangeables;
        else if (value.GetHashCode() > 149 && value.GetHashCode() < 200)
            return ValueTypes.dynamicConstants;
        else if (value.GetHashCode() > 149 && value.GetHashCode() < 250)
            return ValueTypes.command;
        else if (value.GetHashCode() > 249)
        {
            if (value == Values.NULL || value == Values.NONE)
                return ValueTypes.value;
            else if (value == Values.runCommand)
                return ValueTypes.specialValues;
            else if (value == Values.numberValue)
                return ValueTypes.specialValues;
            else if (value == Values.stringValue)
                return ValueTypes.specialValues;
            else if (value == Values.dictStringStore)
                return ValueTypes.specialValues;
            else if (value == Values.dictNameStore)
                return ValueTypes.specialValues;
            else
                return ValueTypes.unknown;
        }
        else
            return ValueTypes.unknown;
    }
    public static float ObjectToFloat(object value)
    {
        if (value is int)
            return (int)value;
        else
            return (float)value;
    }

    public static void Println(string text)
    {
        //Queues up text
        Battler.textToDisplay.Enqueue(text);
    }
}

[Serializable]
public class ExposableMutableValue : MutableValue
{
    public Exposable eValue;

    public ExposableMutableValue(Exposable e)
    {
        eValue = e;
    }

    public ExposableMutableValue()
    {
        eValue = new Exposable();
    }

    public override object GetValue()
    {
        return eValue.value;
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.exposable;
    }

    public void SetValue(float? newValue)
    {
        eValue.value = (newValue.HasValue) ? (float)newValue : eValue.value;
    }
}

[Serializable]
public class StatusMutableValue : MutableValue
{
    public Status sValue;

    public StatusMutableValue(Status s)
    {
        sValue = s;
    }

    public StatusMutableValue()
    {
        sValue = new Status();
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.status;
    }

    public override object GetValue()
    {
        return sValue;
    }
}

[Serializable]
public class PokemonMutableValue : MutableValue
{
    public PokemonMutableValue(Pokemon p, PokemonType t = PokemonType.none)
    {
        pValue = p;
        pType = t;
    }

    public PokemonMutableValue()
    {
        pValue = null;
    }

    public enum PokemonType
    {
        none,
        speed,
        hp,
        maxHP
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.pokemon;
    }

    public override object GetValue()
    {
        switch (pType)
        {
            case PokemonType.hp:
                return pValue.pHp;

            case PokemonType.maxHP:
                return pValue.pMaxHp;

            case PokemonType.speed:
                return pValue.pSpeed;

            case PokemonType.none:
            default:
                return pValue;
        }
    }

    public void SetValue(float? newValue)
    {
        switch (pType)
        {
            case PokemonType.hp:
                pValue.pHp = (int)newValue;
                break;

            case PokemonType.maxHP:
                pValue.pMaxHp = (int)newValue;
                break;

            case PokemonType.speed:
                pValue.pSpeed = (int)newValue;
                break;

            case PokemonType.none:
            default:
                break;
        }
    }
}

[Serializable]
public class ItemMutableValue : MutableValue
{
    public Items iValue;

    public ItemMutableValue(Items i)
    {
        iValue = i;
    }

    public ItemMutableValue()
    {
        iValue = null;
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.item;
    }

    public override object GetValue()
    {
        return iValue;
    }
}

[Serializable]
public class MoveMutableValue : MutableValue
{
    public Move mValue;

    public MoveMutableValue(Move m)
    {
        mValue = m;
    }

    public MoveMutableValue()
    {
        mValue = null;
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.move;
    }

    public override object GetValue()
    {
        return mValue;
    }
}

[Serializable]
public class TypeMutableValue : MutableValue
{
    public Types tValue;

    public TypeMutableValue(Types t)
    {
        tValue = t;
    }

    public TypeMutableValue()
    {
        tValue = null;
    }

    public override TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.type;
    }

    public override object GetValue()
    {
        return tValue;
    }
}

[Serializable]
public class MutableValue
{
    public Pokemon pValue;

    public PokemonMutableValue.PokemonType pType = PokemonMutableValue.PokemonType.none;

    public enum TypeOfMutableValue
    {
        number,
        move,
        item,
        pokemon,
        type,
        status,
        exposable
    }

    public float? vValue;

    public MutableValue(float? tValue)
    {
        vValue = tValue;
    }

    public MutableValue(ref float? tValue)
    {
        vValue = tValue;
    }

    public MutableValue()
    {
        vValue = null;
    }

    public Type GetMutableType()
    {
        return vValue.GetType();
    }

    public virtual TypeOfMutableValue GetTypeOfMutable()
    {
        return TypeOfMutableValue.number;
    }

    public virtual object GetValue()
    {
        return vValue;
    }
}