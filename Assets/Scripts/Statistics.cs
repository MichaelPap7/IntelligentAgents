using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{

    private Text winner_TextComponent;

    private void Awake() 
    {
        winner_TextComponent = GetComponent<WinnerValue>();

        winner_TextComponent.text = "Village 2";
    }
}
