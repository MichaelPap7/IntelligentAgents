using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShowList : MonoBehaviour, IPointerClickHandler
{

    public GameObject Panel;
    public GameObject UIPanel;
    public InputField x;
    public InputField y;
    public InputField treasureAmmount;
    public InputField energyAmmount;
    public InputField energyPrice;
    public InputField mapPrice;
    public InputField agentAmmount;

    public GameObject agents;
    public GameObject agentListB;
    public GameObject agentListA;
    public GameObject blockGameObject;
    public GameObject agent1GameObject;
    public GameObject agent2GameObject;
    public GameObject village1GameObject;
    public GameObject village2GameObject;
    public GameObject energyGameObject;
    public GameObject treasureGameObject;
    public GameObject ironGameObject;
    public GameObject goldGameObject;
    public GameObject cropGameObject;
    public GameObject objectToSpawn;
    public GameObject fogOfWar;

    public GameObject EndScreen;

    public void OpenPanel()
    {
        Debug.Log("Enter1");
        if (Panel != null)
        {
            bool isActive = Panel.activeSelf;

            Panel.SetActive(!isActive);
        }

        if(UIPanel != null)
        {
            Debug.Log("Enter1");
            bool isActive = UIPanel.activeSelf;

            UIPanel.SetActive(!isActive);
            Debug.Log("Enter2");
            
        }
    }
    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        int dimensionX = Int32.Parse(getValueOrPlaceHolder(x));
        int dimensionY = Int32.Parse(getValueOrPlaceHolder(y));
        int treasures = Int32.Parse(getValueOrPlaceHolder(treasureAmmount));
        int energies = Int32.Parse(getValueOrPlaceHolder(energyAmmount));
        int agentNum = Int32.Parse(getValueOrPlaceHolder(agentAmmount));
        int energy_price = Int32.Parse(getValueOrPlaceHolder(energyPrice));
        int map_price = Int32.Parse(getValueOrPlaceHolder(mapPrice));
        GameManager.Setup(dimensionX, dimensionY, treasures, energies, agentNum, energy_price, map_price);
        GameObject worldController = new GameObject();
        GeneratedGrid script = worldController.AddComponent<GeneratedGrid>();
        script.agents = agents;
        script.agentListB = agentListB;
        script.agentListA = agentListA;
        script.blockGameObject = blockGameObject;
        script.agent1GameObject = agent1GameObject;
        script.agent2GameObject = agent2GameObject;
        script.village1GameObject = village1GameObject;
        script.village2GameObject = village2GameObject;
        script.energyGameObject = energyGameObject;
        script.treasureGameObject = treasureGameObject;
        script.ironGameObject = ironGameObject;
        script.goldGameObject = goldGameObject;
        script.cropGameObject = cropGameObject;
        script.objectToSpawn = objectToSpawn;
        script.fogOfWar = fogOfWar;
        script.EndScreen = EndScreen;
        Instantiate(worldController, new Vector3(0, 0, 0), Quaternion.identity);
    }

    #endregion
    private string getValueOrPlaceHolder(InputField item)
    {
        if (String.IsNullOrEmpty(item.text))
        {
            return ((Text)item.placeholder).text;
        }
        return item.text;
    }
}
