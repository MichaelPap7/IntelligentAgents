using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class GameSpeedHandler : MonoBehaviour, IPointerClickHandler
{
    public List<float> Speeds = new List<float>() { 0.5f, 1f, 2f, 5f };
    private int flag = 1;
    public InputField speed;
    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Passed");
        if(flag == Speeds.Count - 1)
        {
            flag = 0;
        }
        else
        {
            flag++;
        }
        speed.text = Speeds[flag].ToString();
        GameManager.GameSpeed = Speeds[flag];
    }

    #endregion
}
