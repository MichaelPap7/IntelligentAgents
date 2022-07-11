using UnityEngine;
using System.Collections.Concurrent;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;

public class GeneratedGrid : MonoBehaviour
{

    public GameObject blockGameObject;
    public GameObject objectToSpawn;

    public ConcurrentDictionary<Tuple<int, int>, Cube> Field = new ConcurrentDictionary<Tuple<int, int>, Cube>();

    public int worldSizeX = 40;
    public int worldSizeZ = 40;
    private int noiseHeight = 5;

    private float gridOffset = 1.1f;

    private List<Vector3> blockPositions = new List<Vector3>();

    void Start()
    {
        Field = GameManager.Field;
        for(int x = 0; x < GameManager.Width; x++) {
            for(int z = 0; z < GameManager.Height; z++) {

                Vector3 pos = new Vector3(x * gridOffset, generateNoise(x,z,8f) * noiseHeight, z * gridOffset);
                //Manage to give GameObject the right Asset depend on the resource
                switch(GameManager.Field[Tuple.Create(x, z)].value)
                {
                    case PlaceContent.Wood:
                        break;
                    case PlaceContent.Crop:
                        break;
                    case PlaceContent.Steel:
                        break;
                    case PlaceContent.Gold:
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
                GameManager.Field[Tuple.Create(x, z)].Position = block.transform.position;


                block.transform.SetParent(this.transform);
            }
        }
        SpawnObject();
        GameManager.StartGame();
    }

    private void SpawnObject () {
        for(int c = 0; c < 20; c++){

            GameObject toPlaceObject = Instantiate(objectToSpawn, ObjectSpawnLocation(), Quaternion.identity);
        }
    }

    private Vector3 ObjectSpawnLocation () {
        int rndIndex = UnityEngine.Random.Range(0, blockPositions.Count);

        Vector3 newPos = new Vector3 (blockPositions[rndIndex].x, blockPositions[rndIndex].y + 1f, blockPositions[rndIndex].z);

        blockPositions.RemoveAt(rndIndex);
        return newPos;

    }

    private float generateNoise (int x, int z, float detailScale) {

        float xNoise = (x + this.transform.position.x) / detailScale;
        float zNoise = (z + this.transform.position.y) / detailScale;

        return Mathf.PerlinNoise(xNoise, zNoise);
    }
}
