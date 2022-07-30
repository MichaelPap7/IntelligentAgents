using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{

    private Text winner_TextComponent;
    public GameObject winnerText;

    private void Awake() 
    {
        winner_TextComponent = winnerText.GetComponent<Text>();

        winner_TextComponent.text = "Village 2";
    }
}
