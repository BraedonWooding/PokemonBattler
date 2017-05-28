using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GraphicMod : MonoBehaviour
{
    [SerializeField]
    Sprite
        sprite1;
    [SerializeField]
    Sprite
        sprite2;
    [SerializeField]
    Toggle
        toggle;
    ModHandler modHolder;
    int temp;

    public void ValueChanged()
    {
        if (toggle.isOn)
        {
            gameObject.GetComponentsInChildren<Image>()[1].sprite = sprite1;
        }
        else
        {
            gameObject.GetComponentsInChildren<Image>()[1].sprite = sprite2;
            if (modHolder == null)
                modHolder = GameObject.Find("_SCRIPTS_STARTUP_").GetComponent<ModHandler>();
        }
        int.TryParse(toggle.gameObject.name, out temp);
        switch (toggle.tag)
        {
            case "Function":
                modHolder.ChangeValue<Function>(temp, toggle.isOn);
                break;
            case "Pokemon":
                modHolder.ChangeValue<Pokemon>(temp, toggle.isOn);
                break;
            case "Item":
                modHolder.ChangeValue<Items>(temp, toggle.isOn);
                break;
            case "Move":
                modHolder.ChangeValue<Move>(temp, toggle.isOn);
                break;
            case "Type":
                modHolder.ChangeValue<Types>(temp, toggle.isOn);
                break;
            case "Status":
                modHolder.ChangeValue<Status>(temp, toggle.isOn);
                break;
            case "Scenario":
                modHolder.ChangeValue<Scenario>(temp, toggle.isOn);
                break;
            default:
                break;
        }

    }
}