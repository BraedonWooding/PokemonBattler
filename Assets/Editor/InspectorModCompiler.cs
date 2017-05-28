using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(ModCompiler))]
public class InspectorModCompiler : Editor
{
    [ExecuteInEditMode()]
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ModCompiler myScript = (ModCompiler)target;
        if (GUILayout.Button("Print Values"))
        {
            Debug.Log("Values");
            for (int i = 0; i < myScript.values.Count; i++)
            {
                Debug.Log(myScript.values[i].GetValue());
            }

            Debug.Log("Dictionary Values"); ;
            for (int i = 0; i < myScript.storeValues.Count; i++)
            {
                Debug.Log(myScript.storeValues.ElementAt(i));
            }
        }
    }
}