using UnityEngine;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class GeneratedGrid : MonoBehaviour
{

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
    
    public bool IsStarted = false;

    public ConcurrentDictionary<Tuple<int, int>, Cube> Field = new ConcurrentDictionary<Tuple<int, int>, Cube>();

    public int worldSizeX = 40;
    public int worldSizeZ = 40;
    private int noiseHeight = 5;

    private float gridOffset = 1.1f;

    private List<Vector3> blockPositions = new List<Vector3>();

    void Start()
    {   
        // GameManager.Width = 40;
        // GameManager.Height = 40;
        // GameManager.VillageA_Supplies = new Supplies() { Crop_Supplies = 8, Gold_Supplies = 8, Steel_Supplies = 8, Wood_Supplies = 8 };
        // GameManager.VillageB_Supplies = new Supplies() { Crop_Supplies = 8, Gold_Supplies = 8, Steel_Supplies = 8, Wood_Supplies = 8 };
        // GameManager.GenerateField();
        //Agent1 agent = new Agent1();
        //GameObject test = Instantiate(blockGameObject, new Vector3(1,2,3), Quaternion.identity) as GameObject;
        //test.AddComponent(typeof(Agent1));
        //var script = test.GetComponent(typeof(Agent1));
        //script = agent;
        GameManager.Setup(worldSizeX, worldSizeZ, 1, 2, 5, new Supplies() { Crop_Supplies = 8, Gold_Supplies = 8, Steel_Supplies = 8, Wood_Supplies = 8 }, new Supplies() { Crop_Supplies = 8, Gold_Supplies = 8, Steel_Supplies = 8, Wood_Supplies = 8 });
        Debug.Log(GameManager.Field.Count);
        
        for(int x = 0; x < GameManager.Width; x++) {
            for(int z = 0; z < GameManager.Height; z++) {                        
                Vector3 pos = new Vector3(x * gridOffset, generateNoise(x,z,30f) * noiseHeight, z * gridOffset);  
                Vector3 rpos = new Vector3(pos.x, pos.y + .5f, pos.z); 
                //Manage to give GameObject the right Asset depend on the resource
                switch(GameManager.Field[Tuple.Create(x, z)].value)
                {
                    case PlaceContent.Wood:
                        SpawnObject(rpos, objectToSpawn);
                        break;
                    case PlaceContent.Crop:
                        SpawnObject(rpos, cropGameObject);
                        break;
                    case PlaceContent.Steel:
                        SpawnObject(rpos, ironGameObject);
                        break;
                    case PlaceContent.Gold:
                        SpawnObject(rpos, goldGameObject);
                        break;
                    case PlaceContent.Vilage1:
                        break;
                    case PlaceContent.Vilage2:
                        break;
                    case PlaceContent.Empty:
                        break;
                    case PlaceContent.Treasure:
                        break;
                    case PlaceContent.Energy:
                        break;
                }
                GameObject block = Instantiate(blockGameObject, pos, Quaternion.identity) as GameObject;

                blockPositions.Add(block.transform.position);
                //Check we want the position of the villager not the actual position of Cube
                GameManager.Field[Tuple.Create(x, z)].coord_x = rpos.x;
                GameManager.Field[Tuple.Create(x, z)].coord_y = rpos.y;
                GameManager.Field[Tuple.Create(x, z)].coord_z = rpos.z;


                block.transform.SetParent(this.transform);
            }
        }
        //Prepare Agent GameObjects
        foreach (var ag1 in GameManager.AgentsV1)
        {
            GameObject temp = Instantiate(agent1GameObject, new Vector3(GameManager.Field[Tuple.Create(ag1.Position.Item1, ag1.Position.Item2)].coord_x, GameManager.Field[Tuple.Create(ag1.Position.Item1, ag1.Position.Item2)].coord_y, GameManager.Field[Tuple.Create(ag1.Position.Item1, ag1.Position.Item2)].coord_z), Quaternion.identity) as GameObject;
            ag1.Transform = temp;
        }
        foreach (var ag2 in GameManager.AgentsV2)
        {
            GameObject temp1 = Instantiate(agent2GameObject, new Vector3(GameManager.Field[Tuple.Create(ag2.Position.Item1, ag2.Position.Item2)].coord_x, GameManager.Field[Tuple.Create(ag2.Position.Item1, ag2.Position.Item2)].coord_y, GameManager.Field[Tuple.Create(ag2.Position.Item1, ag2.Position.Item2)].coord_z), Quaternion.identity) as GameObject;
            ag2.Transform = temp1;
        }
        GameManager.GameSpeed = 1.0f;
    }

    void Update() {
        if(!IsStarted){
            GameManager.StartGame();
            IsStarted = true;
        }
        
        
    }

    private void SpawnObject (Vector3 pos, GameObject spawnObject) {

        GameObject toPlaceObject = Instantiate(spawnObject, pos, Quaternion.identity);
    }

    private Vector3 ObjectSpawnLocation () {
        int rndIndex = UnityEngine.Random.Range(0, blockPositions.Count);

        Vector3 newPos = new Vector3 (blockPositions[rndIndex].x, blockPositions[rndIndex].y + 5f, blockPositions[rndIndex].z);

        blockPositions.RemoveAt(rndIndex);
        return newPos;

    }

    private float generateNoise (int x, int z, float detailScale) {

        float xNoise = (x + this.transform.position.x) / detailScale;
        float zNoise = (z + this.transform.position.y) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }
}
