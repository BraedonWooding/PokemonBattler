using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScenarioLog : MonoBehaviour
{
    public Text title;
    public Text eventTitle;
    public Text eventMessage;

    private static GameObject prefabButton;

    [SerializeField]
    private GameObject holderPrefab;

    public static Scenario currentScenario;
    public static ScenarioNode currentNode;
    public static int nodeIndex = 0;

    void Awake()
    {
        prefabButton = Resources.Load<GameObject>("Prefab_NodeButton");
        nodeIndex = 0;
    }

    void Start()
    {
        //Example
        XMLSerializerScenarios load = XMLSerializerScenarios.Load(Application.dataPath + "/BattlerData/Modding/Scenario/GeneralScenario/ExampleScenario/ExScenario.xml");
        currentScenario = load.scenario;

        if (currentScenario.sSelectUnits)
        {
            Application.LoadLevel("Starting");
        }

        currentNode = currentScenario.sNodes[nodeIndex];

        HandleNewScene();
    }

    public void HandleNewScene()
    {
        title.color = currentScenario.sTitleColor;
        eventTitle.color = currentNode.nTitleColor;
        eventMessage.color = currentNode.nMessageColor;

        title.text = currentScenario.sName;

        eventTitle.text = currentNode.nTitle;
        eventMessage.text = currentNode.nMessage;

        if (currentNode.nHasButtons)
            for (int i = 0; i < currentNode.nButtons.Count; i++)
            {
                GameObject go = Instantiate(prefabButton);
                go.transform.SetParent(holderPrefab.transform);
                go.name = currentNode.nButtons[i].bName;
                ScenarioButton sb = go.GetComponent<ScenarioButton>();
                sb.title = currentNode.nButtons[i].bTitle;
                sb.action = currentNode.nButtons[i].bAction;
                sb.button = currentNode.nButtons[i];
                sb.log = this;
                Button theButton = go.GetComponent<Button>();
                ColorBlock theColor = theButton.colors;
                Color col = currentNode.nButtons[i].bColor;
                theColor.normalColor = col;
                col.a = 0.75f;
                theColor.highlightedColor = col;
                col.a = 0.5f;
                theColor.pressedColor = col;
                theButton.colors = theColor;
            }
    }

    public void NextNode()
    {
        nodeIndex++;
        currentNode = currentScenario.sNodes[nodeIndex];
    }

    public void NodeString(string name)
    {
        ScenarioNode newNode = currentScenario.sNodes.Find(x => x.nName == name);
        if (newNode.nName != "")
            currentNode = newNode;

        HandleNewScene();
    }

    public void NodeInt(int id)
    {
        ScenarioNode newNode = currentScenario.sNodes[id];
        if (newNode.nName != "")
            currentNode = newNode;

        HandleNewScene();
    }
}