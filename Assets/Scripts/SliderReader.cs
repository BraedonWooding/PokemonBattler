using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class SliderReader : MonoBehaviour {

    public Slider slider;
    public Text text;
    public string prefix;
    public string postfix;
    [SerializeField]
    private typeOfSlider type;
    public List<UserType> userTypes = new List<UserType>();

    [Serializable]
    public class UserType
    {
        public int id;
        public string display;
        public bool replaceAllText;
    }

    private enum typeOfSlider {
        integer,
        decimal1,
        decimal2,
        fullDecimal,
        userType
    }
	
	void Update () {

        string value;

        switch (type)
        {
            case typeOfSlider.decimal1:
                value = slider.value.ToString("#0.0");
                break;

            case typeOfSlider.decimal2:
                value = slider.value.ToString("#0.00");
                break;

            case typeOfSlider.fullDecimal:
                value = slider.value.ToString();
                break;

            case typeOfSlider.integer:
                value = ((int)slider.value).ToString();
                break;

            case typeOfSlider.userType:
                UserType user = userTypes.Find(i => i.id == (int)slider.value);
                if (user.replaceAllText)
                {
                    text.text = user.display;
                    return;
                }
                else
                {
                    value = user.display;
                }

                break;

            default:
                value = slider.value.ToString();
                break;
        }

        text.text = prefix + value + postfix;
	}
}