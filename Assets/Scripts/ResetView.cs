using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ResetView : MonoBehaviour
{
   public void OnPointerClick(PointerEventData eventData)
    {
        GameObject.Find("Spawn");
    }
}
