using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowList : MonoBehaviour
{

    public GameObject Panel;
    public GameObject UIPanel;
    
    public void OpenPanel()
    {
        if(Panel != null)
        {
            bool isActive = Panel.activeSelf;

            Panel.SetActive(!isActive);
        }

        if(UIPanel != null)
        {
            bool isActive = UIPanel.activeSelf;

            UIPanel.SetActive(!isActive);
        }
    }
}
