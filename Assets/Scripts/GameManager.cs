using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO; 
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

public static class GameManager
{
    public static ConcurrentDictionary<Tuple<int, int>, Cube> Field = new ConcurrentDictionary<Tuple<int, int>, Cube>();
    public static ConcurrentDictionary<Tuple<int, int>, Cube> DefaultA = new ConcurrentDictionary<Tuple<int, int>, Cube>();
    public static ConcurrentDictionary<Tuple<int, int>, Cube> DefaultB = new ConcurrentDictionary<Tuple<int, int>, Cube>();
    public static ConcurrentDictionary<Tuple<int, int>, GameObject> Resources = new ConcurrentDictionary<Tuple<int, int>, GameObject>();
    public static List<Agent1> AgentsV1 = new List<Agent1>();
    public static List<Agent1> AgentsV2 = new List<Agent1>();
    public static Supplies VillageA_Supplies = new Supplies();
    public static Supplies VillageB_Supplies = new Supplies();    
    public static GeneratedGrid Script;
    
    //InputValues
    public static int Width;
    public static int Height;
    public static int TreasuresCount;
    public static int EnergyFountainsCount;
    public static int NumberOfAgents;
    public static float GameSpeed;
    public static int energy_pot_price = 3;
    public static int map_price = 3;
    public static int energy_pot_restoration = 10;

    //public bool IsStarted = false;
    //public bool IsCompleted = false;

    public static bool end = false;

    //Stastics
    public static long MovesDone;
    public static string Winner;
    public static long TotalTime;
    //VillageA_Supplies
    //VillageB_Supplies
    public static void Setup(int x, int y, int treasures, int energies, int number_agents,int energyPotPrice, int mapPrice)
    {
        Width = x;
        Height = y;
        TreasuresCount = treasures;
        EnergyFountainsCount = energies;
        NumberOfAgents = number_agents;
        energy_pot_price = energyPotPrice;
        map_price = mapPrice;
        int counter = 0;
        int max = x / 35;
        if(y / 50 > max)
        {
            max = y / 50;
        }
        max = max == 0 ? 10 : max;
        VillageA_Supplies = new Supplies() { Crop_Supplies = 20*max, Gold_Supplies = 20 * max, Steel_Supplies = 20 * max, Wood_Supplies = 20 * max };
        VillageB_Supplies = new Supplies() { Crop_Supplies = 20 * max, Gold_Supplies = 20 * max, Steel_Supplies = 20 * max, Wood_Supplies = 20 * max };
        energy_pot_restoration = max * energy_pot_restoration;
        GenerateField(x, y, treasures, energies, number_agents * 2);
        foreach (var key in Field.Keys)
        {
            DefaultA[key] = MaskValue(Field[key]).DeepClone();
            DefaultB[key] = MaskValue(Field[key]).DeepClone();
            if (Field[key].value == PlaceContent.Vilage1)
            {
                DefaultA[key] = Field[key].DeepClone();
            }
            else if (Field[key].value == PlaceContent.Vilage2)
            {
                DefaultB[key] = Field[key].DeepClone();
            }
            else if (Field[key].value == PlaceContent.Energy)
            {
                DefaultA[key] = Field[key].DeepClone();
                DefaultB[key] = Field[key].DeepClone();
            }
        }
        int Acounter = 1;
        int Bcounter = 1;
        for (int i = 0; i < number_agents; i++)
        {
            int patch = number_agents / 4;
            if (i < patch)
            {
                AgentsV1.Add(GetAgentV1(PlaceContent.Crop, $"A{Acounter}"));
                Acounter++;
            }
            else if (i < patch * 2)
            {
                AgentsV1.Add(GetAgentV1(PlaceContent.Steel, $"A{Acounter}"));
                Acounter++;
            }
            else if (i < patch * 3)
            {
                AgentsV1.Add(GetAgentV1(PlaceContent.Wood, $"A{Acounter}"));
                Acounter++;
            }
            else if (i < patch * 4)
            {
                AgentsV1.Add(GetAgentV1(PlaceContent.Gold, $"A{Acounter}"));
                Acounter++;
            }
            else
            {
                AgentsV1.Add(GetAgentV1(PlaceContent.Crop, $"A{Acounter}"));
                Acounter++;
            }
            AgentsV2.Add(GetAgentV2($"B{Bcounter}"));
            Bcounter++;
        }
    }
    /*
     public static void GenerateAgents(GameObject asset1,GameObject asset2)
    {

        foreach(var ag1 in AgentsV1)
        {
            GameObject temp = Instantiate(asset1, new Vector3(Field[Tuple.Create(ag1.Position.Item1,ag1.Position.Item2)].coord_x, Field[Tuple.Create(ag1.Position.Item1, ag1.Position.Item2)].coord_y, Field[Tuple.Create(ag1.Position.Item1, ag1.Position.Item2)].coord_z), Quaternion.identity) as GameObject;
            temp.AddComponent(typeof(Agent1));
            var script = temp.GetComponent(typeof(Agent1));
            script = ag1;
            AgentsV1Obj.Add(temp);
        }
        foreach (var ag1 in AgentsV1)
        {
            GameObject temp1 = Instantiate(asset2, new Vector3(1, 2, 3), Quaternion.identity) as GameObject;
            temp1.AddComponent(typeof(Agent1));
            var script1 = temp1.GetComponent(typeof(Agent1));
            script1 = ag1;
            AgentsV1Obj.Add(temp1);
        }
    }
    */
    public static void DestroyResource(Tuple<int,int> pos){
        Script.DestroyResource(pos);
    }
    public static string StartGame()
    {

        while (!end)
        {
            Thread.Sleep(GameSpeed == 1 ? 1000 : GameSpeed == 0.5 ? 2000 : GameSpeed == 2 ? 500 : GameSpeed == 5? 100 : 10);
            Debug.Log("Agents Run");
            for (int i = 0; i < AgentsV1.Count - 1; i++)
            {

                AgentsV1[i].Step();
                if (!AgentsV2[i].Stopped)
                {
                    AgentsV2[i].Step();
                    //AgentsV2[i].PrintFieldView();
                }

            }
            //if (!AgentsV2[0].Stopped)
            //{
            //    AgentsV2[0].Step();
            //    AgentsV2[0].PrintFieldView();
            //}
            Debug.Log("CheckEndgame");
            if (EndGame() == "A")
            {
                return "VillageA";
            }
            if (EndGame() == "B")
            {
                return "VillageB";
            }
            if (Field.Values.All(x => x.Quantity == 0) && EndGame() != "A" && EndGame() != "B")
            {
                Console.WriteLine("VillageA");
                Console.WriteLine("Crop" + VillageA_Supplies.Crop_Supplies.ToString());
                Console.WriteLine("Wood" + VillageA_Supplies.Wood_Supplies.ToString());
                Console.WriteLine("Steel" + VillageA_Supplies.Steel_Supplies.ToString());
                Console.WriteLine("Gold" + VillageA_Supplies.Gold_Supplies.ToString());

                Console.WriteLine("VillageB");
                Console.WriteLine("Crop" + VillageB_Supplies.Crop_Supplies.ToString());
                Console.WriteLine("Wood" + VillageB_Supplies.Wood_Supplies.ToString());
                Console.WriteLine("Steel" + VillageB_Supplies.Steel_Supplies.ToString());
                Console.WriteLine("Gold" + VillageB_Supplies.Gold_Supplies.ToString());

                return "Unknown";
            }
        }
        return "Unknown";
    }

    public static string EndGame()
    {
        if (VillageA_Supplies.Steel_Supplies <= 0 && VillageA_Supplies.Wood_Supplies <= 0 && VillageA_Supplies.Crop_Supplies <= 0 && VillageA_Supplies.Gold_Supplies <= 0)
            return "A";
        if (VillageB_Supplies.Steel_Supplies <= 0 && VillageB_Supplies.Wood_Supplies <= 0 && VillageB_Supplies.Crop_Supplies <= 0 && VillageB_Supplies.Gold_Supplies <= 0)
            return "B";
        return "N";
    }

    public static Agent1 GetAgentV1(PlaceContent role, string name)
    {
        Agent1 agent1 = new Agent1();

        agent1.Name = name;
        agent1.Field = DefaultA;
        agent1.Role = role;
        agent1.Position = Tuple.Create(Field.First(x => x.Value.value == PlaceContent.Vilage1).Value.x, Field.First(x => x.Value.value == PlaceContent.Vilage1).Value.y);
        agent1.Energy = 900;
        agent1.Balance = 25;
        agent1.Energy_Pots = 3;
        agent1.Speed = 1;

        return agent1;
    }
    public static Agent1 GetAgentV2(string name)
    {
        Agent1 agent1 = new Agent1();

        agent1.Name = name;
        agent1.Field = DefaultB;
        agent1.Role = PlaceContent.Empty;
        agent1.Position = Tuple.Create(Field.First(x => x.Value.value == PlaceContent.Vilage2).Value.x, Field.First(x => x.Value.value == PlaceContent.Vilage2).Value.y);
        agent1.Energy = 900;
        agent1.Balance = 25;
        agent1.Energy_Pots = 3;
        agent1.Speed = 2;

        return agent1;
    }
    public static Dictionary<Tuple<int, int>, Cube> InputToField(List<Cube> Cubes)
    {
        Dictionary<Tuple<int, int>, Cube> result = new Dictionary<Tuple<int, int>, Cube>();
        foreach (var cube in Cubes)
        {
            result[Tuple.Create(cube.x, cube.y)] = cube;
        }
        return result;
    }
    public static string FieldToOutput(Dictionary<Tuple<int, int>, Cube> field)
    {
        List<Cube> result = new List<Cube>();
        foreach (var key in field.Keys)
        {
            result.Add(field[key]);
        }
        return JsonUtility.ToJson(result);;
    }
    public static void GenerateField(int x, int y, int treasures, int energies, int agents_number)
    {
        System.Random random = new System.Random();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (i < 5 && j < 5)
                {
                    Field[Tuple.Create(i, j)] = new Cube() { x = i, y = j, value = PlaceContent.Vilage1, Quantity = 0 };
                }
                else if (i > x - 5 && j > y - 5)
                {
                    Field[Tuple.Create(i, j)] = new Cube() { x = i, y = j, value = PlaceContent.Vilage2, Quantity = 0 };
                }
                else
                {
                    Field[Tuple.Create(i, j)] = new Cube() { x = i, y = j, value = PlaceContent.Empty, Quantity = 0 };
                }

            }
        }
        for (int i = 0; i < treasures; i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Treasure;
            Field[rand_place].Quantity = map_price * 20;
        }

        for (int i = 0; i < energies; i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Energy;
            Field[rand_place].Quantity = energy_pot_price * agents_number * 4;
        }
        for (int i = 0; i < random.Next(y) * x; i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Crop;
            Field[rand_place].Quantity = VillageA_Supplies.Crop_Supplies * 3;
        }
        for (int i = 0; i < random.Next(y) * x; i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Wood;
            Field[rand_place].Quantity = VillageA_Supplies.Wood_Supplies * 3;
        }
        for (int i = 0; i < random.Next(y) * (x - 1); i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Steel;
            Field[rand_place].Quantity = VillageA_Supplies.Steel_Supplies * 2;
        }
        for (int i = 0; i < random.Next(y) * (x - 1); i++)
        {
            var empties = Field.Where(x => x.Value.value == PlaceContent.Empty).Select(x => x.Key).ToList().DeepClone();
            var rand_place = empties[random.Next(empties.Count)].DeepClone();
            Field[rand_place].value = PlaceContent.Gold;
            Field[rand_place].Quantity = VillageA_Supplies.Gold_Supplies * 2;
        }
        //Print
        //Dictionary<int, string> values = new Dictionary<int, string>();
        //foreach (var key in Field.Keys)
        //{
        //    if (values.ContainsKey(key.Item2))
        //        values[key.Item2] += Field[key].value.ToString() + $"({Field[key].Quantity})";
        //    else
        //        values[key.Item2] = Field[key].value.ToString() + $"({Field[key].Quantity})";
        //}
        //foreach (var row in values.Keys)
        //{
        //    Console.WriteLine(values[row]);
        //}


    }

    private static Cube MaskValue(Cube cube)
    {
        var result = new Cube();
        result.x = cube.x;
        result.y = cube.y;
        result.value = PlaceContent.UnKnown;
        return result;
    }

    public static T DeepClone<T>(this T obj)
    {
        using (var ms = new MemoryStream())
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            ms.Position = 0;

            return (T)formatter.Deserialize(ms);
        }
    }
}