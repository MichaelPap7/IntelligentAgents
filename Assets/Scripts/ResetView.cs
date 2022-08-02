using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ResetView : MonoBehaviour
{
    public Button ResetButton;
    public void Start()
    {
        ResetButton.onClick.AddListener(OnPointerClick);
    }
   public void OnPointerClick()
    {
        GameObject controller = GameObject.Find("Spawn");
        GeneratedGrid script = controller.GetComponent(typeof(GeneratedGrid)) as GeneratedGrid;
        script.PointOfView("AB");
    }
}
