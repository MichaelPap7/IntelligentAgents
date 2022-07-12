using System.Collections;
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class Agent1
{
    public ConcurrentDictionary<Tuple<int, int>, Cube> Field;
    public Tuple<int, int> Position;
    public PlaceContent Role;
    public string Name;
    public bool Stopped = false;
    public int Energy = 0;
    public int Balance = 0;
    public int Energy_Pots = 0;
    public int Speed = 1;

    private Action LastAction = Action.MoveToSource;
    private Tuple<int, int> Waypoint;
    private PlaceContent Cargo = PlaceContent.Empty;

    public GameObject Transform;
    
    public void Step()
    {
        var action = Observe();
        switch (action)
        {
            case Action.MoveToSource:
                MoveToSource();
                break;
            case Action.ReturnToVillage:
                ReturnToVillage();
                break;
            case Action.MoveToEnergy:
                MoveToEnergy();
                break;
            case Action.BuyEnergy:
                BuyEnergy();
                break;
            case Action.Collect:
                Collect();
                break;
            case Action.BuyMapInfo:
                BuyMapInfo();
                break;
            case Action.Deposit:
                Deposit();
                break;
        }
        Energy--;
        if (Energy == 0)
        {
            if (Energy_Pots > 0)
            {
                Energy_Pots--;
                Energy += GameManager.energy_pot_restoration;
            }
            else
            {
                Stopped = true;
                Destroy(Transform);
            }
        }


    }

    private void Deposit()
    {
        switch (Cargo)
        {
            case PlaceContent.Wood:
                if (Role == PlaceContent.Empty)
                {
                    GameManager.VillageB_Supplies.Wood_Supplies--;
                }
                else
                {
                    GameManager.VillageA_Supplies.Wood_Supplies--;
                }
                break;
            case PlaceContent.Crop:
                if (Role == PlaceContent.Empty)
                {
                    GameManager.VillageB_Supplies.Crop_Supplies--;
                }
                else
                {
                    GameManager.VillageA_Supplies.Crop_Supplies--;
                }

                break;
            case PlaceContent.Steel:
                if (Role == PlaceContent.Empty)
                {
                    GameManager.VillageB_Supplies.Steel_Supplies--;
                }
                else
                {
                    GameManager.VillageA_Supplies.Steel_Supplies--;
                }

                break;
            case PlaceContent.Gold:
                if (Role == PlaceContent.Empty)
                {
                    GameManager.VillageB_Supplies.Gold_Supplies--;
                }
                else
                {
                    GameManager.VillageA_Supplies.Gold_Supplies--;
                }

                break;
        }
        Cargo = PlaceContent.Empty;
    }

    private void BuyMapInfo()
    {
        var agent = FindNearbyAgent();

        var field = agent.BuyMap(this);
        if (field == null)
        {
            if (Cargo == PlaceContent.Empty)
            {
                MoveToSource();
            }
            else
            {
                ReturnToVillage();
            }
        }
        else
        {
            MergeKnowledge(field);
        }



    }

    private void MergeKnowledge(ConcurrentDictionary<Tuple<int, int>, Cube> field)
    {
        foreach (var key in field.Keys)
        {
            if (Field[key].value == PlaceContent.UnKnown || Field[key].Quantity > field[key].Quantity)
                Field[key] = field[key].DeepClone();
        }
    }

    private ConcurrentDictionary<Tuple<int, int>, Cube> BuyMap(Agent1 agent1)
    {
        int counter = 0;
        foreach (var key in agent1.Field.Keys)
        {
            if (Field[key].value == PlaceContent.UnKnown || Field[key].Quantity > agent1.Field[key].Quantity)
                counter++;
        }
        if (counter > 7 || (counter > 0 && (Balance < GameManager.map_price || Balance < GameManager.energy_pot_price)))
        {
            agent1.Balance -= GameManager.map_price;
            Balance += GameManager.map_price;
            return Field;
        }
        return null;
    }

    private Agent1 FindNearbyAgent()
    {
        List<Tuple<int, int>> places = PlacesToCheck();
        foreach (var place in places)
        {
            if (Field.ContainsKey(place))
            {
                var agent = IsAgentInPlace(place);
                if (agent != null)
                {
                    return agent;
                }

            }
        }
        return null;
    }

    private Agent1 IsAgentInPlace(Tuple<int, int> place)
    {
        foreach (var agent in GameManager.AgentsV1)
        {
            if (agent.Position.Equals(place)) { return agent; }
        }
        foreach (var agent in GameManager.AgentsV2)
        {
            if (agent.Position.Equals(place)) { return agent; }
        }
        return null; ;
    }

    private List<Tuple<int, int>> PlacesToCheck()
    {
        List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();
        tuples.Add(Tuple.Create(Position.Item1 - 1, Position.Item2 - 1));
        tuples.Add(Tuple.Create(Position.Item1 - 1, Position.Item2));
        tuples.Add(Tuple.Create(Position.Item1, Position.Item2 - 1));
        tuples.Add(Tuple.Create(Position.Item1 + 1, Position.Item2));
        tuples.Add(Tuple.Create(Position.Item1, Position.Item2 + 1));
        tuples.Add(Tuple.Create(Position.Item1 + 1, Position.Item2 + 1));
        tuples.Add(Tuple.Create(Position.Item1 + 1, Position.Item2 - 1));
        tuples.Add(Tuple.Create(Position.Item1 - 1, Position.Item2 + 1));
        return tuples;
    }

    private void Collect()
    {
        if (Role != PlaceContent.Empty ? Field[Position].value == Role : IsOre(Field[Position].value) && Field[Position].Quantity > 0)
        {
            Cargo = Field[Position].value;
            GameManager.Field[Position].Quantity--;
            Field[Position].Quantity--;
        }
        if (IsEnergyThirsty() && Field[Position].value == PlaceContent.Energy)
        {
            GameManager.Field[Position].Quantity--;
            Field[Position].Quantity--;
            Energy += Field.Count / 2;
        }
    }

    private bool IsOre(PlaceContent value)
    {
        if (value == PlaceContent.Gold || value == PlaceContent.Steel || value == PlaceContent.Crop || value == PlaceContent.Wood)
        {
            return true;
        }
        return false;
    }

    private void BuyEnergy()
    {
        bool succeed = TryBuyEnergy();
        if (succeed)
            return;
        if (Cargo == PlaceContent.Empty)
        {
            MoveToSource();
        }
        else
        {
            if (Role != PlaceContent.Empty ? Field[Position].value == PlaceContent.Vilage1 : Field[Position].value == PlaceContent.Vilage2)
            {
                Deposit();
            }
            else
            {
                ReturnToVillage();
            }

        }
    }

    private void MoveToEnergy()
    {
        bool succeed = TryBuyEnergy();
        if (succeed)
            return;
        var energy_place = FindTarget(Action.MoveToEnergy);
        if (energy_place.Equals(Position))
        {
            Collect();
            return;
        }
        Move(energy_place);
    }

    private void ReturnToVillage()
    {
        var village_place = FindTarget(Action.ReturnToVillage);
        Move(village_place);
    }

    private void MoveToSource()
    {
        var source_place = FindTarget(Action.MoveToSource);
        Move(source_place);
    }

    private bool TryBuyEnergy()
    {
        var agent = FindNearbyAgent();
        Agent1 agent1 = new Agent1();

        agent1 = (Agent1)agent;
        if (agent1 == null)
        {
            return false;
        }
        bool succeed = agent1.BuyEnergy(this);
        if (succeed)
        {
            if (Energy < 2)
            {
                UseEnergyPot();
            }
            return true;
        }


        return false;
    }

    private void UseEnergyPot()
    {
        if (Energy_Pots > 0)
        {
            Energy += GameManager.energy_pot_restoration;
            Energy_Pots--;
        }
    }

    private bool BuyEnergy(Agent1 agent1)
    {
        if (Energy_Pots > 1 && Energy > 20)
        {
            Energy_Pots--;
            agent1.Energy_Pots++;
            Balance += GameManager.energy_pot_price;
            agent1.Balance -= GameManager.energy_pot_price;
            return true;
        }
        return false;
    }

    private void Move(Tuple<int, int> target)
    {
        int counter = Speed.DeepClone();
        //Match x
        int remainingx = Waypoint.Item1 - Position.Item1;
        while (Waypoint.Item1 != Position.Item1 && counter != 0)
        {
            if (remainingx < 0)
            {
                Position = Tuple.Create(Position.Item1 - 1, Position.Item2);
                Transform.transform.position = new Vector3(GameManager.Field[Position].coord_x,GameManager.Field[Position].coord_y,GameManager.Field[Position].coord_z);
                counter--;
            }
            else
            {
                Position = Tuple.Create(Position.Item1 + 1, Position.Item2);
                Transform.transform.position = new Vector3(GameManager.Field[Position].coord_x,GameManager.Field[Position].coord_y,GameManager.Field[Position].coord_z);
                counter--;
            }

        }
        //Match y
        int remainingy = Waypoint.Item2 - Position.Item2;
        if (Waypoint.Item2 != Position.Item2 && counter != 0)
        {
            if (remainingy < 0)
            {
                Position = Tuple.Create(Position.Item1, Position.Item2 - 1);
                Transform.transform.position = new Vector3(GameManager.Field[Position].coord_x,GameManager.Field[Position].coord_y,GameManager.Field[Position].coord_z);
                counter--;
            }
            else
            {
                Position = Tuple.Create(Position.Item1, Position.Item2 + 1);
                Transform.transform.position = new Vector3(GameManager.Field[Position].coord_x,GameManager.Field[Position].coord_y,GameManager.Field[Position].coord_z);
                counter--;
            }
        }
    }

    private Tuple<int, int> FindTarget(Action action)
    {
        switch (action)
        {
            case Action.MoveToEnergy:
                return FindShortestPath(PlaceContent.Energy);
                break;
            case Action.ReturnToVillage:
                return Role != PlaceContent.Empty ? FindShortestPath(PlaceContent.Vilage1) : FindShortestPath(PlaceContent.Vilage2);
                break;
            case Action.MoveToSource:
                return FindShortestPath(PlaceContent.Empty);
                break;
        }
        return null;
    }

    private Tuple<int, int> FindShortestPath(PlaceContent desiredPlace)
    {
        if (desiredPlace != PlaceContent.Empty)
        {
            var desiredPlaces = Field.Values.Where(x => x.value == desiredPlace).ToList();
            if (desiredPlaces.Count == 0)
                return FindShortestPath(PlaceContent.Empty);
            var place = desiredPlaces.Where(x => (Math.Abs(Position.Item1 - x.x) + Math.Abs(Position.Item2 - x.y)) == desiredPlaces.Min(x => Math.Abs(Position.Item1 - x.x) + Math.Abs(Position.Item2 - x.y))).ToList();
            Waypoint = Tuple.Create(place[0].x, place[0].y);
            return Tuple.Create(place[0].x, place[0].y);
        }
        else
        {
            if (Waypoint != null && LastAction == Action.MoveToSource && !Waypoint.Equals(Position))
            {
                return Waypoint;
            }
            var ores = Field.Values.Where(x => Role != PlaceContent.Empty ? x.value == Role : IsOre(x.value) && x.Quantity > 0).ToList();
            if (ores.Count > 0)
            {
                var place = ores.Where(x => (Math.Abs(Position.Item1 - x.x) + Math.Abs(Position.Item2 - x.y)) == ores.Min(x => Math.Abs(Position.Item1 - x.x) + Math.Abs(Position.Item2 - x.y))).ToList();
                Waypoint = Tuple.Create(place[0].x, place[0].y);
                return Tuple.Create(place[0].x, place[0].y);
            }
            System.Random random = new System.Random();
            Tuple<int, int> way = Tuple.Create(0, 0);
            do
            {
                var unknownplaces = Field.Values.Where(x => x.value == PlaceContent.UnKnown).ToList();
                if (unknownplaces.Count == 0)
                {
                    Waypoint = Tuple.Create(-1, -1);
                    Stopped = true;
                    return Waypoint;
                }

                var chosenPlace = unknownplaces[random.Next(unknownplaces.Count)].DeepClone();


                way = Tuple.Create(chosenPlace.x, chosenPlace.y);
            } while (way.Equals(Position) || Field[way].value != PlaceContent.UnKnown);
            Waypoint = way;
            return Waypoint;
        }
    }

    private Action Observe()
    {
        //Check Neighbour positions
        List<Tuple<int, int>> places = PlacesToCheck();
        foreach (var place in places)
        {
            if (Field.ContainsKey(place))
            {
                Field[place] = GameManager.Field[place].DeepClone();
            }
        }

        var action = Action.MoveToSource;
        if (IsEnergyThirsty())
        {
            action = Action.MoveToEnergy;
        }
        else if (CanCollect())
        {
            action = Action.Collect;
        }
        else if (IsToBuyMapInfo())
        {
            action = Action.BuyMapInfo;
        }
        else if (IsToBuyEnergy())
        {
            action = Action.BuyEnergy;
        }
        else if (Cargo != PlaceContent.Empty && (Role != PlaceContent.Empty ? Field[Position].value == PlaceContent.Vilage1 : Field[Position].value == PlaceContent.Vilage2))
        {
            action = Action.Deposit;
        }
        else if (Cargo != PlaceContent.Empty)
        {
            action = Action.ReturnToVillage;
        }
        else
        {
            action = Action.MoveToSource;
        }
        return action;
    }

    private bool IsToBuyEnergy()
    {
        var agent = FindNearbyAgent();
        return (agent != null ? agent.Energy_Pots > 0 : false) && Balance > GameManager.energy_pot_price;
    }

    private bool IsToBuyMapInfo()
    {
        var agent = FindNearbyAgent();
        if (agent == null)
            return false;
        int counter = 0;
        foreach (var key in agent.Field.Keys)
        {
            if ((Field[key].value != agent.Field[key].value && Field[key].value == PlaceContent.UnKnown) || Field[key].Quantity != agent.Field[key].Quantity)
                counter++;
        }
        if (counter > 7 || (counter > 0 && (Balance > GameManager.map_price || Balance > GameManager.energy_pot_price)))
        {
            return true;
        }
        return false;
    }

    private bool CanCollect()
    {
        return (Role != PlaceContent.Empty ? Field[Position].value == Role : IsOre(Field[Position].value) && Field[Position].Quantity > 0 && Cargo == PlaceContent.Empty) || (Field[Position].value == PlaceContent.Energy && IsEnergyThirsty());

    }

    private bool IsEnergyThirsty()
    {
        return Waypoint != null && CalculateSteps() > Energy;
    }

    private int CalculateSteps()
    {
        if (Field.Where(x => x.Value.value == PlaceContent.Energy && x.Value.Quantity > 0).Count() == 0)
            return Int32.MaxValue;
        var energy_place = Field.First(x => x.Value.value == PlaceContent.Energy && x.Value.Quantity > 0);
        int steps = ((Math.Abs(Waypoint.Item1 - Position.Item1) + Math.Abs(Waypoint.Item2 - Position.Item2)) + (energy_place.Value != null ? Math.Abs(Waypoint.Item1 - energy_place.Key.Item1) + Math.Abs(Waypoint.Item2 - energy_place.Key.Item2) : 0)) / Speed;
        return steps;
    }
    public void PrintFieldView()
    {
        //Print
        Dictionary<int, string> values = new Dictionary<int, string>();
        foreach (var key in Field.Keys)
        {
            if (values.ContainsKey(key.Item2))
                values[key.Item2] += Field[key].value.ToString().Substring(0, 3) + $"({Field[key].Quantity})";
            else
                values[key.Item2] = Field[key].value.ToString().Substring(0, 3) + $"({Field[key].Quantity})";
        }
        foreach (var row in values.Keys)
        {
            Console.WriteLine(values[row]);
        }
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine(Environment.NewLine);
        Console.WriteLine(Environment.NewLine);
    }
}