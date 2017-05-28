using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToggleReader : MonoBehaviour {

    public Toggle toggle;
    public Text text;
    public string prefix;
    public string postfix;
    public string ifTrue = "True";
    public string ifFalse = "False";
	
	// Update is called once per frame
	void Update () {
        text.text = prefix + ((toggle.isOn) ? ifTrue : ifFalse) + postfix;
	}
}