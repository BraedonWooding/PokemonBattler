using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    public int hp;
    public int maxHp;
    private Image img;
    private float percentHp;
    [SerializeField]
    private Text txt;

    void Start()
    {
        img = GetComponent<Image>();
    }

    void Update()
    {
        percentHp = (float)hp / maxHp;
        img.rectTransform.localScale = new Vector3(Mathf.Clamp01(percentHp), 1, 1);
        if (percentHp <= 0.5f)
            if (percentHp <= 0.25f)
                img.color = Color.red;
            else
                img.color = Color.yellow;
        else
            img.color = Color.green;

        txt.text = hp + "/" + maxHp; 
    }
}