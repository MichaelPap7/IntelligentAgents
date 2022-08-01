using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{

    private Text winner_TextComponent;
    public GameObject winnerText;

    private Text time_TextComponent;
    public GameObject timeText;

    private Text moves_TextComponent;
    public GameObject movesText;

    private Text info_TextComponent;
    public GameObject infoText;


    private void Awake() 
    {
        winner_TextComponent = winnerText.GetComponent<Text>();

        winner_TextComponent.text = GameManager.Winner;

        time_TextComponent = timeText.GetComponent<Text>();

        time_TextComponent.text = GameManager.TotalTime.ToString();

        moves_TextComponent = movesText.GetComponent<Text>();

        moves_TextComponent.text = GameManager.MovesDone.ToString();

        info_TextComponent = infoText.GetComponent<Text>();

        info_TextComponent.text = GameManager.TotalInfoExchanged.ToString();
    }
}
