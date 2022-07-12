using UnityEngine;
using System;
[Serializable]

public class Cube
{
    public int x { get; set; }
    public int y { get; set; }
    public PlaceContent value { get; set; }

    public int Quantity { get; set; }

    public float coord_x { get; set; }
    public float coord_y { get; set; }
    public float coord_z { get; set; }
}