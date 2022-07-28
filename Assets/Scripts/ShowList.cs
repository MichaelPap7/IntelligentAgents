using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShowList : MonoBehaviour
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

            GameManager.Setup(Int32.Parse(x.text), Int32.Parse(y.text), Int32.Parse(treasureAmmount.text), Int32.Parse(energyAmmount.text), Int32.Parse(agentAmmount.text),Int32.Parse(energyPrice.text),Int32.Parse(mapPrice.text));
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
            Instantiate(worldController,new Vector3(0,0,0),Quaternion.identity);
        }
    }
}
